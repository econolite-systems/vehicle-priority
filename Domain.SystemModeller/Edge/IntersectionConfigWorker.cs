// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.OdeRepository.SystemModeller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.SystemModeller.Edge;

public class IntersectionConfigWorker : BackgroundService
{
    private readonly IEntityNodeJsonFileRepository _entityNodeJsonFileRepository;
    private readonly ISink<int, EntityNodeConfigRequest> _sink;
    private readonly ISource<int, GenericJsonResponse> _source;
    private readonly ILogger<IntersectionConfigWorker> _logger;
    private readonly IMetricsFactory _metricsFactory;
    private readonly UserEventFactory _userEventFactory;
    private readonly int _intersection;

    public IntersectionConfigWorker(
        IConfiguration configuration,
        IEntityNodeJsonFileRepository entityNodeJsonFileRepository,
        ISink<int, EntityNodeConfigRequest> sink,
        ISource<int, GenericJsonResponse> source,
        ILogger<IntersectionConfigWorker> logger,
        IMetricsFactory metricsFactory,
        UserEventFactory userEventFactory
        )
    {
        _entityNodeJsonFileRepository = entityNodeJsonFileRepository;
        _sink = sink;
        _source = source;
        _logger = logger;
        _metricsFactory = metricsFactory;
        _userEventFactory = userEventFactory;
        _intersection = configuration.GetValue("Intersection", 0);
        if (_intersection == 0)
        {
            throw new NullReferenceException("Intersection missing in config.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting to consume config request");
        try
        {
            await _sink.SinkAsync(_intersection, new EntityNodeConfigRequest(), stoppingToken);
            await _source.ConsumeOnAsync(async result =>
            {
                if (result.Key == _intersection)
                {
                    await HandleConfigAsync(result, stoppingToken);
                }
            }, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Ending config consumption");
        }
    }

    private async Task HandleConfigAsync(ConsumeResult<int, GenericJsonResponse> result, CancellationToken stoppingToken)
    {
        var value = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(result.Value.Json, JsonPayloadSerializerOptions.Options);
        if (value != null)
        {
            await _entityNodeJsonFileRepository.SaveJsonAsync(value.ToArray());
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Edge configs received: {0}", value.Count())));
        }
    }
}