// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Repository.VehiclePriority;

public class VehicleSmoothingRepo : IVehicleSmoothingRepo
{
    public VehicleSmoothingRepo()
    {
        
    }
    
    public Task<Vehicle> UpdateVehicle(VehicleUpdate update)
    {
        var result = update.ToVehicle();
        
        return Task.FromResult(result);
    }
}
