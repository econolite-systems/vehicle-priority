// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Persistence.Common.Interfaces;
using Econolite.Ode.Persistence.Common.Records;

namespace Econolite.Ode.Models.VehiclePriority.Status;

public record VehicleLocationStatus(string Id, string Type, string Name, float Latitude, float Longitude, int Direction, float Speed, string Tag, long Timestamp, bool Stop = false) : IndexedRecordBase<string>(Id);

public static class VehicleLocationStatusExtensions
{
    public static VehicleLocationStatus ToLocationStatus(this VehicleUpdate update, DateTime dateTime)
    {
        return new VehicleLocationStatus(update.VehicleId, update.VehicleType, update.VehicleName,
            float.Parse(update.VehicleLatitude), float.Parse(update.VehicleLongitude), update.TravelDirection,
            (float)update.TravelSpeed, update.Tag, dateTime.ToFileTimeUtc());
    }
    
    public static VehicleLocationStatus ToStop(this VehicleLocationStatus update)
    {
        return new VehicleLocationStatus(update.Id, update.Type, update.Name,
            update.Latitude, update.Longitude, update.Direction,
            (float)update.Speed, update.Tag, update.Timestamp, true);
    }
}
