// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Persistence.Common.Records;

namespace Econolite.Ode.Models.VehiclePriority.Api;

public record PriorityRequestVehicleConfiguration(string Id, IEnumerable<PriorityRequestVehicle> Vehicles, IEnumerable<PriorityRequestVehicleClassType> PriorityRequestVehicleClassType, IEnumerable<PriorityRequestVehicleClassLevel> PriorityRequestVehicleClassLevel, IEnumerable<PriorityRequestVehicleIntersection>? PriorityRequestVehicleIntersections) : IndexedRecordBase<string>(Id);

public record PriorityRequestVehicle(string Id, string Type, bool Allowed, ScheduleTime? StartTime, ScheduleTime? EndTime);

public record PriorityRequestVehicleClassType(int Id, string Type);

public record PriorityRequestVehicleClassLevel(int Id, string Type);

public record PriorityRequestVehicleIntersection(Guid Id, string Name, bool Disable, IEnumerable<PriorityRequestVehicleName> Vehicles);

public record PriorityRequestVehicleName(string Name);

public record ScheduleTime(int Hour, int Minute);

public static class PriorityRequestVehicleExtension
{
    public static PriorityRequestVehicle FlipAllowed(this PriorityRequestVehicle vehicle)
    {
        return vehicle with { Allowed = !vehicle.Allowed };
    }
    
    public static bool ShouldRun(this PriorityRequestVehicle vehicle, DateTime current)
    {
        return vehicle.EndTime != null && vehicle.StartTime != null && vehicle.StartTime.ShouldRun(vehicle.EndTime, current);
    }
    
    public static bool ShouldRun(this ScheduleTime start, ScheduleTime end, DateTime current)
    {
        var startTime = start.ToDateTime();
        var endTime = end.ToDateTime();
        
        return current >= startTime && current < endTime;
    }
    
    public static DateTime ToDateTime(this ScheduleTime time)
    {
        var current = DateTime.Now;
        var scheduleTime = new DateTime(current.Year, current.Month, current.Day, time.Hour, time.Minute, 0, DateTimeKind.Local);
        return scheduleTime;
    }

    public static PriorityRequestVehicleConfiguration UpdateIntersections(
        this PriorityRequestVehicleConfiguration config, IEnumerable<EntityNode> signals)
    {
        return new PriorityRequestVehicleConfiguration(config.Id, config.Vehicles,
            config.PriorityRequestVehicleClassType, config.PriorityRequestVehicleClassLevel,
            config.PriorityRequestVehicleIntersections.Update(signals));
    }
    
    public static IEnumerable<PriorityRequestVehicleIntersection> ToPriorityRequestVehicleIntersections(
        this IEnumerable<EntityNode> signals)
    {
        return signals.Select(s => s.ToPriorityRequestVehicleIntersection());
    }
    
    public static IEnumerable<PriorityRequestVehicleIntersection> Update(
        this IEnumerable<PriorityRequestVehicleIntersection>? prvIntersections, IEnumerable<EntityNode> signals)
    {
        if (prvIntersections == null)
        {
            return signals.ToPriorityRequestVehicleIntersections();
        }
        return signals.Select(s =>
        {
            var result = prvIntersections.FirstOrDefault(i => i.Id == s.Id);
            return result != null ? result.Update(s) : s.ToPriorityRequestVehicleIntersection();
        });
    }
    
    public static PriorityRequestVehicleIntersection ToPriorityRequestVehicleIntersection(this EntityNode signal)
    {
        return new PriorityRequestVehicleIntersection(signal.Id, signal.Name, true, Array.Empty<PriorityRequestVehicleName>());
    }

    public static PriorityRequestVehicleIntersection Update(this PriorityRequestVehicleIntersection prv,
        EntityNode signal)
    {
        return prv with { Id = signal.Id, Name = signal.Name };
    }
}
