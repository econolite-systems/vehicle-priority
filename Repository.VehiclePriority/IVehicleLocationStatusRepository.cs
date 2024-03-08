// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Status;

namespace Econolite.Ode.Repository.VehiclePriority;

public interface IVehicleLocationStatusRepository
{
    Task UpdateStatus(VehicleLocationStatus status);
    Task<IEnumerable<VehicleLocationStatus>> RemoveOldStatusAsync();
}
