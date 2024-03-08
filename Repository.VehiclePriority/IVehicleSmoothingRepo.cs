// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Repository.VehiclePriority;

public interface IVehicleSmoothingRepo
{
    Task<Vehicle> UpdateVehicle(VehicleUpdate update);
}
