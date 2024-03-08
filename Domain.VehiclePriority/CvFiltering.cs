// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Repository.VehiclePriority;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.VehiclePriority;

public class CvFiltering : ICvFiltering
{
    private readonly IPriorityRequestVehicleEdgeRepository _priorityRequestVehicleEdgeRepository;

    public CvFiltering(IPriorityRequestVehicleEdgeRepository priorityRequestVehicleEdgeRepository)
    {
        _priorityRequestVehicleEdgeRepository = priorityRequestVehicleEdgeRepository;
    }
    
    public async Task<bool> ProcessVehicleAsync(string id)
    {
        return await _priorityRequestVehicleEdgeRepository.IsVehicleAllowedAsync(id);
    }
}
