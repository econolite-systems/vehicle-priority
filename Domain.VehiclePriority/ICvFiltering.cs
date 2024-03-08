// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Domain.VehiclePriority;

public interface ICvFiltering
{
    Task<bool> ProcessVehicleAsync(string id);
}
