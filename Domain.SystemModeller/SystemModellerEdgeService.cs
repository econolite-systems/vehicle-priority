// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Helpers.Exceptions;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.Entities.Types;
using Econolite.OdeRepository.SystemModeller;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.Configuration;

public class SystemModellerEdgeService : ISystemModellerService
{
    private readonly ILogger<SystemModellerEdgeService> _logger;
    private readonly IEntityNodeJsonFileRepository _repository;

    public SystemModellerEdgeService(IEntityNodeJsonFileRepository repository, ILogger<SystemModellerEdgeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<EntityNode>> GetAllAsync()
    {
        return await _repository.GetAllExceptDeletedAsync();
    }
    
    public async Task<IEnumerable<EntityNode>> GetAllByIntersectionIdAsync(Guid id)
    {
        return await _repository.GetByIntersectionIdAsync(id);
    }

    public async Task<EntityNode?> GetByIdAsync(Guid id)
    {
        return (await _repository.GetByIntersectionIdAsync(id)).ToArray().FirstOrDefault(e => e.Type.Id == IntersectionTypeId.Id || e.Type.Id == SignalTypeId.Id);
    }

    public async Task<EntityNode?> AddAsync(EntityNode add)
    {
        return await Task.FromResult(add);
    }

    public async Task<EntityNode?> UpdateAsync(EntityNode update)
    {
        return await Task.FromResult(update);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await Task.FromResult(false);
    }
    
    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        return await Task.FromResult(false);
    }
    
    public async Task<bool> SoftDeleteByIntersectionAsync(Guid id)
    {
        return await Task.FromResult(false);
    }
    
    public async Task PublishConfigAsync(Guid id)
    {
        await Task.FromResult(false);
    }
}
