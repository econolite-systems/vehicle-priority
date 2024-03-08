// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.

using System.Text.Json;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority.Cloud;

public class IntersectionConfigResponseWorker : BackgroundService
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly ISink<PriorityResponseConfigurationMessage> _sink;
    private readonly ISource<int, EntityNodeConfigRequest> _source;
    private readonly ILogger<IntersectionConfigResponseWorker> _logger;
    private readonly IMetricsFactory _metricsFactory;
    private readonly UserEventFactory _userEventFactory;
    private readonly IEntityService _entityService;

    public IntersectionConfigResponseWorker(
        ISink<PriorityResponseConfigurationMessage> sink,
        ISource<int, EntityNodeConfigRequest> source,
        ILogger<IntersectionConfigResponseWorker> logger,
        IMetricsFactory metricsFactory,
        UserEventFactory userEventFactory,
        IServiceProvider serviceProvider
        )
    {
        _sink = sink;
        _source = source;
        _logger = logger;
        _metricsFactory = metricsFactory;
        _userEventFactory = userEventFactory;
        _vehiclePriorityService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IVehiclePriorityService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to consume config request");
        try
        {
            await _source.ConsumeOnAsync(async result =>
            {
                await HandleConfigAsync(result, stoppingToken);
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending config consumption");
        }
    }

    private async Task HandleConfigAsync(ConsumeResult<int, EntityNodeConfigRequest> result, CancellationToken stoppingToken)
    {
        await PublishConfigAsync();
    }

    private async Task PublishConfigAsync()
    {
        var results = await _vehiclePriorityService.GetAllPriorityRequestVehicleClassesAsync();
        var json = JsonSerializer.Serialize(results, JsonPayloadSerializerOptions.Options);
        var response = new PriorityResponseConfigurationMessage(json);
        await _sink.SinkAsync(Guid.Empty, response, CancellationToken.None);
    }
}