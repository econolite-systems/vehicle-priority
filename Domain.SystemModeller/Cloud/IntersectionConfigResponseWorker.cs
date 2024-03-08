// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.SystemModeller.Cloud;

public class IntersectionConfigResponseWorker : BackgroundService
{
    private readonly ISink<int, GenericJsonResponse> _sink;
    private readonly ISource<int, EntityNodeConfigRequest> _source;
    private readonly ILogger<IntersectionConfigResponseWorker> _logger;
    private readonly IMetricsFactory _metricsFactory;
    private readonly UserEventFactory _userEventFactory;
    private readonly IEntityService _entityService;

    public IntersectionConfigResponseWorker(
        IConfiguration configuration,
        ISink<int, GenericJsonResponse> sink,
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
        _entityService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IEntityService>();
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
        await PublishConfigAsync(result.Key);
    }

    private async Task PublishConfigAsync(int id)
    {
        var intersectionEntities = await _entityService.GetByIntersectionIdMapAsync(id);
        var results = intersectionEntities.ToArray();
        var json = JsonSerializer.Serialize(results, JsonPayloadSerializerOptions.Options);
        var response = new EntityNodeJsonConfigResponse(json);
        await _sink.SinkAsync(id, response, CancellationToken.None);
    }
}