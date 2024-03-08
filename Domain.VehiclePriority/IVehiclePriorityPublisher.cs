// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Domain.VehiclePriority;

public interface IVehiclePriorityPublisher
{
    Task PublishVehicleUpdateAsync(VehicleUpdate update);
    Task PublishEtaAsync(RouteStatus routeStatus);
    Task PublishConfigAsync(PriorityRequestVehicleConfiguration config);
}
