// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Models.VehiclePriority;

public static class RouteStatusExtensions
{
    public static RouteStatus ToRouteStatus(this PriorityRequest request)
    {
        return new RouteStatus
        {
            Id = Guid.Parse(request.RequestId),
            RouteId = request.RouteId != null ? Guid.Parse(request.RouteId) : Guid.Empty,
            DesiredClassLevel = request.DesiredClassLevel,
            VehicleId = request.VehicleId,
            VehicleType = request.VehicleType,
            VehicleName = request.VehicleName,
            Tag = request.Tag,
            Metadata = request.RequestMetadata,
            LastUpdate = DateTime.Now
        };
    }

    public static RouteStatus ToCancelled(this RouteStatus status, DateTime current)
    {
        status.Completed = true;
        status.EndTime = current;
        status.EtaInSeconds = 0;
        status.Eta = status.EndTime;
        status.PreviousIntersection = status.NextIntersection;
        status.DesiredClassLevel = status.DesiredClassLevel;
        status.LastUpdate = current;

        return status;
    }
}
