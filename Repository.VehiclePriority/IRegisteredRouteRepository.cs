// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Common.Repository;

namespace Econolite.Ode.Repository.VehiclePriority;

public interface IRegisteredRouteRepository : IRepository<RegisteredRoute, Guid>
{
}
