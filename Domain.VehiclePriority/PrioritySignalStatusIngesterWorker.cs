// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.SystemModeller;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Status;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class PrioritySignalStatusIngesterWorker: BackgroundService
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly VehiclePriorityVehicleStatusHub _hub;
    private readonly IConsumer<Guid, PriorityStatusMessage> _consumer;
    private readonly ILogger<PrioritySignalStatusIngesterWorker> _logger;
    private readonly IMetricsCounter _loopCounter;
    private readonly UserEventFactory _userEventFactory;
    private readonly IEntityConfigurationService _entityConfigurationService;

    public PrioritySignalStatusIngesterWorker(IConfiguration configuration, IServiceProvider serviceProvider, VehiclePriorityVehicleStatusHub hub, IConsumer<Guid, PriorityStatusMessage> consumer, ILogger<PrioritySignalStatusIngesterWorker> logger, IMetricsFactory metricsFactory, UserEventFactory userEventFactory)
    {
        var topic = configuration[Consts.TOPIC_ODE_VEHICLE_STATUS] ?? Consts.TOPIC_ODE_VEHICLE_STATUS_DEFAULT;
        _hub = hub;
        _consumer = consumer;
        _logger = logger;
        _userEventFactory = userEventFactory;
        consumer.Subscribe(topic);
        _loopCounter = metricsFactory.GetMetricsCounter("Signal Status");
        using var scope = serviceProvider.CreateScope();
        _vehiclePriorityService = scope.ServiceProvider.GetRequiredService<IVehiclePriorityService>();
        _entityConfigurationService = scope.ServiceProvider.GetRequiredService<IEntityConfigurationService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(1);

            var timer = new Timer(Callback, null, startTimeSpan, periodTimeSpan);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Guid, PriorityStatusMessage>? result = null;

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

                        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, string.Format("Unhandled exception while processing signal status: {0}", result?.Type)));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("System modeler config worker stopping");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Exception thrown while processing priority status messages");
            }
        });
    }

    private async void Callback(object? _)
    {
        await CheckForStaleStatusesAsync();
    }

    private async Task ParserAsync(ConsumeResult<Guid, PriorityStatusMessage> result, DateTime dateTime)
    {
        if (!result.DeviceId.HasValue) return;
        var entities = await _entityConfigurationService.GetIntersectionByIdAsync(result.DeviceId.Value);
        var entity = entities.FirstOrDefault(entity => entity.Type.Id == SignalTypeId.Id);
        if (entity == null)
        {
            _logger.LogCritical("Entity {EntityId} does not exist", result.DeviceId.ToString());
            return;
        }
        var name = entity.Name;

        var propertiesModel = entity.Geometry?.Point;
        
        if (propertiesModel == null)
        {
            _logger.LogCritical("Entity {EntityId} does not have a geometry", result.DeviceId.ToString());
            return;
        }
        
        var status = result.Value.ToStatus(result.Key, name, propertiesModel, dateTime);
        await _vehiclePriorityService.UpdatePriorityStatusAsync(status);
        await _hub.SendSignalUpdateAsync(status);

        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Signal status received: {0}", status.Name ?? status.Id.ToString())));
    }

    private async Task CheckForStaleStatusesAsync()
    {
        var statuses = (await _vehiclePriorityService.RemoveOldPriorityStatusAsync()).ToList();

        if (statuses.Any())
        {
            foreach (var status in statuses)
            {
                await _hub.SendSignalUpdateAsync(status.ToOffline());
            }

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Signal status marked as stale: {0}", statuses.Count)));
        }
    }
}
