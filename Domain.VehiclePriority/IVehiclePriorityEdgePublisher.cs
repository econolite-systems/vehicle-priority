// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Asn1J2735.J2735;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Domain.VehiclePriority;

public interface IVehiclePriorityEdgePublisher
{
    Task PublishPrsStatusAsync(PriorityStatusMessage update);
    Task PublishPrsResponseAsync(PriorityResponseMessage update);
    Task PublishVehicleUpdateAsync(VehicleUpdate update);
    Task PublishEtaAsync(RouteStatus routeStatus);
    Task PublishSsmAsync(SignalStatusMessage signalStatus);
}
