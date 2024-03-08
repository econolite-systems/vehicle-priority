// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Status;

namespace Econolite.Ode.Domain.VehiclePriority;

public interface IVehiclePriorityService
{
    Task<bool> RegisterHeartbeatAsync(string sourceid);
    Task<PriorityRequest?> RequestPriorityAsync(PriorityRequest request);
    Task<bool> CancelPriorityRequestAsync(string requestId);
    Task<string?> RegisterRouteAsync(RouteUpdate route);
    Task<string?> UpdateRouteAsync(RouteUpdate route);
    Task UpdateVehicleAsync(VehicleUpdate update);
    Task UpdateVehicleLocationStatusAsync(VehicleLocationStatus status);
    Task<IEnumerable<VehicleLocationStatus>> RemoveOldVehicleStatusAsync();
    Task UpdatePriorityStatusAsync(RoutePriorityStatus status);
    Task<IEnumerable<RoutePriorityStatus>> RemoveOldPriorityStatusAsync();
    Task<PriorityRequestVehicleConfiguration> GetAllPriorityRequestVehicleClassesAsync();
    Task<string?> AddPriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config);
    Task<string?> UpdatePriorityRequestVehicleClassesAsync(PriorityRequestVehicleConfiguration config);
}
