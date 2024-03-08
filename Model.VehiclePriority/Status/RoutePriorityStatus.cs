// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Persistence.Common.Records;

namespace Econolite.Ode.Models.VehiclePriority.Status;

public record RoutePriorityStatus(Guid Id, string Type, string Name, float Latitude, float Longitude, string Status, IEnumerable<PriorityStatus> Prs, long Timestamp) : IndexedRecordBase<Guid>(Id);

public static class RoutePriorityStatusExtensions
{
    
    public static RoutePriorityStatus ToStatus(this PriorityStatusMessage update, Guid id, string name, GeoJsonPointFeature point, DateTime timestamp)
    {
        var status = update.PriorityStatus.Any(s => s.PriorityRequestStatus == (int) PriorityRequestStatus.ActiveProcessing) ? "Priority" : "Online";
        var prs = update.PriorityStatus;
        return new RoutePriorityStatus(id, "Signal", name,
            (float) point.Coordinates[1], (float) point.Coordinates[0], status, prs, timestamp.ToFileTimeUtc());
    }

    public static RoutePriorityStatus ToOnline(this RoutePriorityStatus update)
    {
        return new RoutePriorityStatus(update.Id, update.Type, update.Name,
            update.Latitude, update.Longitude, "Online", update.Prs, update.Timestamp);
    }
    
    public static RoutePriorityStatus ToOffline(this RoutePriorityStatus update)
    {
        return new RoutePriorityStatus(update.Id, update.Type, update.Name,
            update.Latitude, update.Longitude, "Offline", update.Prs, update.Timestamp);
    }
    
    public static RoutePriorityStatus ToPriority(this RoutePriorityStatus update)
    {
        return new RoutePriorityStatus(update.Id, update.Type, update.Name,
            update.Latitude, update.Longitude, "Priority", update.Prs, update.Timestamp);
    }
}
