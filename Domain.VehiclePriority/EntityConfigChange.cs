// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Helpers.Exceptions;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Interfaces;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.VehiclePriority.Config;
using Microsoft.Extensions.Configuration;

namespace Econolite.Ode.Domain.VehiclePriority;

public class EntityConfigUpdate : IEntityConfigUpdate
{
    private readonly IProducer<Guid, GenericJsonResponse> _producer;
    private readonly IMessageFactory<Guid, GenericJsonResponse> _messageFactory;
    private readonly string _producerTopic;

    public EntityConfigUpdate(IConfiguration configuration, IProducer<Guid, GenericJsonResponse> producer, IMessageFactory<Guid, GenericJsonResponse> messageFactory)
    {
        _producer = producer;
        _messageFactory = messageFactory;
        _producerTopic = configuration["Topics:ConfigPriorityResponse"] ?? throw new NullReferenceException("Topics:ConfigPriorityResponse missing in config.");
    }
    
    public async Task Add(IEntityService service, EntityNode entity)
    {
        var intersection = GetIntersectionProperty(entity);
        if (!intersection.HasValue) return;

        await PublishConfigAsync(service, intersection.Value);
    }

    public async Task Update(IEntityService service, EntityNode entity)
    {
        var intersection = GetIntersectionProperty(entity);
        if (!intersection.HasValue) return;

        await PublishConfigAsync(service, intersection.Value);
    }

    public async Task Delete(IEntityService service, EntityNode entity)
    {
        var intersection = GetIntersectionProperty(entity);
        if (!intersection.HasValue) return;

        await PublishConfigAsync(service, intersection.Value);
    }
    
    public async Task PublishConfigAsync(IEntityService service, Guid id)
    {
        var entities = await service.GetByIntersectionIdAsync(id);
        var results = entities.Select(e => e.ToEntityModel());
        var json = JsonSerializer.Serialize<IEnumerable<EntityModel>>(results, JsonPayloadSerializerOptions.Options);
        await _producer.ProduceAsync(TopicWithDeviceId(id), _messageFactory.Build(id, new EntityNodeJsonConfigResponse(json)));
    }
    
    private string TopicWithDeviceId(Guid id)
    {
        return $"{_producerTopic}.{id}";
    }
    
    private Guid? GetIntersectionProperty(EntityNode entity)
    {
        if (entity.Geometry.Point?.Properties?.Intersection != null)
        {
            return entity.Geometry.Point.Properties.Intersection.Value;
        }
        if (entity.Geometry.Polygon?.Properties?.Intersection != null)
        {
            return entity.Geometry.Polygon.Properties.Intersection.Value;
        }
        if (entity.Geometry.LineString?.Properties?.Intersection != null)
        {
            return entity.Geometry.LineString.Properties.Intersection.Value;
        }

        return null;
    }
}
