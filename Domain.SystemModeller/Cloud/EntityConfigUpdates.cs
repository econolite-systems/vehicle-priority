// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Interfaces;
using Econolite.Ode.Models.VehiclePriority.Config;
using Microsoft.Extensions.Configuration;

namespace Econolite.Ode.Domain.SystemModeller.Cloud;

public class EntityConfigUpdate : IEntityConfigUpdate
{
    private readonly ISink<int, GenericJsonResponse> _configurationProducer;
    private readonly Guid _tenantId;

    public EntityConfigUpdate(IServiceProvider serviceProvider, ISink<int, GenericJsonResponse> producer, IConfiguration configuration)
    {
        _configurationProducer = producer;
        _tenantId = Guid.Parse(configuration["TenantId"] ?? "Unknown");
    }
    
    public async Task Add(IEntityService service, EntityNode entity)
    {
        await SendConfigurationAsync(service, entity);
    }

    public async Task Update(IEntityService service, EntityNode entity)
    {
        await SendConfigurationAsync(service, entity);
    }

    public async Task Delete(IEntityService service, EntityNode entity)
    {
        await SendConfigurationAsync(service, entity);
    }
    
    private async Task SendConfigurationAsync(IEntityService service, EntityNode entity)
    {
        var id = entity.ToIntersectionId();
        if (id == Guid.Empty)
        {
            return;
        }
        var intersectionEntities = await service.GetByIntersectionIdAsync(id);
        var results = intersectionEntities.ToArray();
        var idMapping = results.ToArray().ToIntersectionIdMap();
        if (!idMapping.HasValue)
        {
            return;
        }
        var json = JsonSerializer.Serialize(results.ToArray(), JsonPayloadSerializerOptions.Options);
        var rsus = new EntityNodeJsonConfigResponse(json);
        await _configurationProducer.SinkAsync(idMapping.Value, rsus, CancellationToken.None);
    }
}

public static class EntityConfigUpdateExtensions
{
    public static Guid ToIntersectionId(this EntityNode entity)
    {
        var result = Guid.NewGuid();
        if ( entity.Geometry?.Point?.Properties?.Intersection.HasValue ?? false)
        {
            result = entity.Geometry.Point.Properties.Intersection.Value;
        }
        else if ( entity.Geometry?.LineString?.Properties?.Intersection.HasValue ?? false)
        {
            result = entity.Geometry.LineString.Properties.Intersection.Value;
        }
        else if ( entity.Geometry?.Polygon?.Properties?.Intersection.HasValue ?? false)
        {
            result = entity.Geometry.Polygon.Properties.Intersection.Value;
        }
        return result;
    }
    
    public static int? ToIntersectionIdMap(this IEnumerable<EntityNode> entities)
    {
        return entities
                .Where(x => x.Type.Name is "Intersection" or "Signal")
                .Select(x => x.IdMapping)
                .FirstOrDefault();
    }
}