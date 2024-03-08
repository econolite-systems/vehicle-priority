// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Status;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
//using Econolite.Ode.Repository.VehiclePriority;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityStatusIngesterWorker: BackgroundService
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly VehiclePriorityVehicleStatusHub _hub;
    private readonly IConsumer<Guid, VehicleUpdate> _consumer;
    private readonly ILogger<VehiclePriorityStatusIngesterWorker> _logger;
    private readonly IMetricsCounter _loopCounter;
    private readonly UserEventFactory _userEventFactory;

    public VehiclePriorityStatusIngesterWorker(IConfiguration configuration, IServiceProvider serviceProvider, VehiclePriorityVehicleStatusHub hub, IConsumer<Guid, VehicleUpdate> consumer, ILogger<VehiclePriorityStatusIngesterWorker> logger, IMetricsFactory metricsFactory, UserEventFactory userEventFactory)
    {
        var topic = configuration[Consts.TOPIC_ODE_VEHICLE_UPDATE] ?? Consts.TOPIC_ODE_VEHICLE_UPDATE_DEFAULT;
        _hub = hub;
        _consumer = consumer;
        _logger = logger;
        _userEventFactory = userEventFactory;
        consumer.Subscribe(topic);
        _loopCounter = metricsFactory.GetMetricsCounter("Status");

        using var scope = serviceProvider.CreateScope();
        _vehiclePriorityService = scope.ServiceProvider.GetRequiredService<IVehiclePriorityService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new Timer(async _ =>
            {
                await CheckForStaleStatusesAsync();
            }, null, startTimeSpan, periodTimeSpan);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Guid, VehicleUpdate>? result = null;

                    try
                    {
                        result = _consumer.Consume(stoppingToken);
                        await ParserAsync(result, DateTime.Now);
                        _consumer.Complete(result);

                        _loopCounter.Increment();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Unhandled exception while processing: {@MessageType}", result?.Type);

                        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, string.Format("Unhandled exception while processing status: {0}", result?.Type)));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("System modeler config worker stopping");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while processing vehicle update messages");
            }
        });
    }
    
    private async Task ParserAsync(ConsumeResult<Guid, VehicleUpdate> result, DateTime dateTime)
    {
        var status = result.Value.ToLocationStatus(dateTime);
        await _vehiclePriorityService.UpdateVehicleLocationStatusAsync(status);
        await _hub.SendUpdateAsync(status);

        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Status received: {0}", status.Name ?? status.Id)));
    }

    private async Task CheckForStaleStatusesAsync()
    {
        var statuses = (await _vehiclePriorityService.RemoveOldVehicleStatusAsync()).ToList();

        if (statuses.Any())
        {
            foreach (var status in statuses)
            {
                await _hub.SendUpdateAsync(status.ToStop());
            }

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Status marked as stale: {0}", statuses.Count)));
        }
    }
}
