// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Common.Scheduler.Base.Timers.Impl;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Repository.VehiclePriority;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityEdgeConfigWorker : BackgroundService
{
    private const string PriorityResponseConfigurationMessage = nameof(PriorityResponseConfigurationMessage);
    private readonly IPriorityRequestVehicleEdgeRepository _priorityRequestVehicleEdgeRepository;
    private readonly ISource<PriorityResponseConfigurationMessage> _source;
    private readonly ILogger<VehiclePriorityEdgeConfigWorker> _logger;
    private readonly IMetricsCounter _loopCounter;
    private readonly UserEventFactory _userEventFactory;
    private readonly string _intersection;
    private readonly TopOfMinuteTimer _timer;
    private readonly IMetricsCounter _vehicleConfigCounter;

    public VehiclePriorityEdgeConfigWorker(IConfiguration configuration,
        IPriorityRequestVehicleEdgeRepository priorityRequestVehicleEdgeRepository,
        ISource<PriorityResponseConfigurationMessage> source,
        ILogger<VehiclePriorityEdgeConfigWorker> logger,
        IMetricsFactory metricsFactory,
        UserEventFactory userEventFactory)
    {
        _priorityRequestVehicleEdgeRepository = priorityRequestVehicleEdgeRepository;
        _source = source;
        _logger = logger;
        _userEventFactory = userEventFactory;
        _intersection = configuration["Intersection"] ?? throw new NullReferenceException("Intersection missing in config.");
        _timer = new TopOfMinuteTimer();
        _loopCounter = metricsFactory.GetMetricsCounter("Edge Config");
        _vehicleConfigCounter = metricsFactory.GetMetricsCounter("Edge Config Vehicles");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to consume config request");
        try
        {
            await _source.ConsumeOnAsync(async result =>
            {
                await PriorityResponseConfigurationResponseAsync(result);
            }, stoppingToken);
            _loopCounter.Increment();
        }
        finally
        {
            _logger.LogInformation("Ending config consumption");
        }
    }

    private async Task PriorityResponseConfigurationResponseAsync(ConsumeResult<Guid, PriorityResponseConfigurationMessage> result)
    {
        var config = JsonSerializer.Deserialize<PriorityRequestVehicleConfiguration>(result.Value.Json, JsonPayloadSerializerOptions.Options);
        if (config != null)
        {
            await _priorityRequestVehicleEdgeRepository.SaveJsonAsync(new [] { config });
            await CheckAllowedVehiclesAsync();
        }
    }

    private async Task CheckAllowedVehiclesAsync()
    {
        var current = DateTime.Now;
        var config = await _priorityRequestVehicleEdgeRepository.LoadDataAsync();
        var configList = new List<PriorityRequestVehicleConfiguration>();
        var updateConfig = false;
        foreach (var priorityRequestVehicleConfiguration in config)
        {
            
            var vehicles = new List<PriorityRequestVehicle>();
            foreach (var vehicle in priorityRequestVehicleConfiguration.Vehicles)
            {
                var updatedVehicle = vehicle;
                var allowed = vehicle.ShouldRun(current);
                if ((allowed && !vehicle.Allowed) || (!allowed && vehicle.Allowed))
                {
                    updateConfig = true;
                    updatedVehicle = vehicle.FlipAllowed();
                }
                vehicles.Add(updatedVehicle);
            }

            if (updateConfig)
            {
                var updatedConfig = new PriorityRequestVehicleConfiguration(priorityRequestVehicleConfiguration.Id,
                    vehicles, priorityRequestVehicleConfiguration.PriorityRequestVehicleClassType,
                    priorityRequestVehicleConfiguration.PriorityRequestVehicleClassLevel, null);
                
                configList.Add(updatedConfig);
            }
        }
        
        if (updateConfig)
        {
            _logger.LogInformation("White List Updated");
            await _priorityRequestVehicleEdgeRepository.SaveJsonAsync(configList);

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Added vehicles to whitelist: {0}", configList.Count)));

            _vehicleConfigCounter.Increment(configList.Count);
        }
        
        Console.WriteLine(DateTime.Now.ToString("O"));
    }

    public override void Dispose()
    {
#pragma warning disable VSTHRD110
        Task.Run(async () => await _timer.StopAsync());
#pragma warning restore VSTHRD110
    }
}
