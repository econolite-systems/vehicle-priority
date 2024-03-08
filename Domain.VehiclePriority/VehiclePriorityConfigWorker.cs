// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.SystemModeller;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.Configuration;

public class VehiclePriorityConfigWorker: BackgroundService
{
    private const string EntityModelConfigRequestType = nameof(EntityModelConfigRequestAsync);
    private readonly IConsumer<Guid, EntityNodeConfigRequest> _consumer;
    private readonly ISystemModellerService _systemModellerService;
    private readonly ILogger<VehiclePriorityConfigWorker> _logger;

    public VehiclePriorityConfigWorker(IConfiguration configuration, IConsumer<Guid, EntityNodeConfigRequest> consumer, IServiceProvider serviceProvider, ILogger<VehiclePriorityConfigWorker> logger)
    {
        _consumer = consumer;
        _systemModellerService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISystemModellerService>();
        _logger = logger;
        var topic = configuration["Topics:ConfigPriorityRequest"] ?? "priority.intersection.config.request";
        _consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed topic {@}", topic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);

                try
                {
                    var task = result.Type switch
                    {
                        EntityModelConfigRequestType => EntityModelConfigRequestAsync(result),
                        _ => Task.CompletedTask
                    };

                    await task;
                    _consumer.Complete(result);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Unhandled exception while processing: {@MessageType}", result.Type);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("System modeller config worker stopping");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception thrown while processing priority config request messages");
        }
    }

    private async Task EntityModelConfigRequestAsync(ConsumeResult<Guid, EntityNodeConfigRequest> result)
    {
        if (!result.DeviceId.HasValue)
        {
            return;
        }
        
        await _systemModellerService.PublishConfigAsync(result.DeviceId.GetValueOrDefault());
    }
}
