// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.VehiclePriority;

public interface IRouteStatusRepository : IRepository<RouteStatus, Guid>
{
    Task<RouteStatus?> GetByRouteIdAsync(Guid id);
    Task<RouteStatus?> GetByVehicleIdAsync(string id);
    Task<IEnumerable<RouteStatus>> GetByVehicleAndTripPointAsync(string id, TripPointLocation location);
    Task<int> GetNewTaskId(Guid intersectionId);
}
