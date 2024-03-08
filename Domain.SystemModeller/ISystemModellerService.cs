// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;

namespace Econolite.Ode.Domain.Configuration;

public interface ISystemModellerService
{
    Task<IEnumerable<EntityNode>> GetAllAsync();
    Task<IEnumerable<EntityNode>> GetAllByIntersectionIdAsync(Guid id);
    Task<EntityNode?> GetByIdAsync(Guid id);
    Task<EntityNode?> AddAsync(EntityNode add);
    Task<EntityNode?> UpdateAsync(EntityNode update);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task PublishConfigAsync(Guid id);
}
