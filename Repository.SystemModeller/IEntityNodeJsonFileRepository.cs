// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;

namespace Econolite.OdeRepository.SystemModeller;

public interface IEntityNodeJsonFileRepository
{
    Task<IEnumerable<EntityNode>> GetAllExceptDeletedAsync();
    Task<IEnumerable<EntityNode>> QueryIntersectingGeoFences(GeoJsonPointFeature point);
    Task<IEnumerable<EntityNode>> GetByIntersectionIdAsync(Guid id);
    Task<IEnumerable<EntityNode>> QueryIntersectingIntersections(GeoJsonLineStringFeature route);
    Task<IEnumerable<EntityNode>> QueryIntersectingApproaches(GeoJsonLineStringFeature route);
    Task<IEnumerable<EntityNode>> QueryIntersectingStreetSegment(GeoJsonPointFeature point);
    Task SoftDelete(Guid id);
    Task<IEnumerable<EntityNode>> LoadDataAsync();
    Task SaveJsonAsync(IEnumerable<EntityNode> models);
}
