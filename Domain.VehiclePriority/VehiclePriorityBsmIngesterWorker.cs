// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityBsmIngesterWorker: BackgroundService
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly IConsumer<Guid, OdeBsmData> _consumer;
    private readonly ILogger<VehiclePriorityEdgeIngesterWorker> _logger;

    public VehiclePriorityBsmIngesterWorker(IServiceProvider serviceProvider, IConsumer<Guid, OdeBsmData> consumer, ILogger<VehiclePriorityEdgeIngesterWorker> logger)
    {
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        _vehiclePriorityService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IVehiclePriorityService>();
        _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Guid, OdeBsmData>? result = null;
                    try
                    {
                        result = _consumer.Consume(stoppingToken);
                        await BsmParserAsync(result);
                        _consumer.Complete(result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception while processing: {@MessageType}",  result != null ? result.Type : "Unknown");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Service stopping");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Main processing loop terminated!");
            }
        });
    }
    
    private async Task BsmParserAsync(ConsumeResult<Guid, OdeBsmData> result)
    {
        await _vehiclePriorityService.UpdateVehicleAsync(result.Value.ToVehicleUpdate());
    }
}
