// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Client.VehiclePriority.Model.J2735;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority.J2735;

namespace Econolite.Ode.Models.VehiclePriority.Api
{
    public class VehicleUpdate
    {
        /// <summary>
        ///     ID of the vehicle from remote system.
        /// </summary>
        public string VehicleId { get; set; } = String.Empty;

        /// <summary>
        ///     Type of vehicle. firetruck, bus, snowplow, freight, passengercar.
        /// </summary>
        public string VehicleType { get; set; } = String.Empty;

        /// <summary>
        ///     Name of vehicle from remote system.
        /// </summary>
        public string VehicleName { get; set; } = String.Empty;

        /// <summary>
        ///     Any external data, such as an ID, that the user wishes to track with the request.
        /// </summary>
        public string Tag { get; set; } = String.Empty;

        /// <summary>
        ///     ID of the route the vehicle is on if known, null if not known.  This
        ///     should be populated from the RegisterRoute API.
        /// </summary>
        public string? RouteId { get; set; }

        public string VehicleLatitude { get; set; } = "0";

        public string VehicleLongitude { get; set; } = "0";

        /// <summary>
        ///     A bearing in degrees north being 0-359°. This is not high precision (fives?)
        /// </summary>
        public int TravelDirection { get; set; }

        // 
        /// <summary>
        ///     Current Vehicle speed.
        ///     The units are defined in TransitMaster, mph, kph etc. Centracs will need a matching setting.
        ///     Assume MPH temporarily. Again, not high precision, five mph?
        /// </summary>
        public double TravelSpeed { get; set; }
    }

    public class Vehicle
    {
        public DateTime Timestamp { get; set; }
        public Guid RouteId { get; set; }
        public string Id { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public GeoJsonPointFeature Location { get; set; } = new GeoJsonPointFeature();
        
        public int? Bearing { get; set; }

        public double? Speed { get; set; }

        public bool IsCancelled { get; set; } = false;
    }

    public static class VehicleExtension
    {
        public static Vehicle ToVehicle(this VehicleUpdate update)
        {
            return new Vehicle()
            {
                Timestamp = DateTime.Now,
                RouteId = update.RouteId == null? Guid.Empty : Guid.Parse(update.RouteId),
                Id = update.VehicleId,
                Type = update.VehicleType,
                Name = update.VehicleName,
                Location = new GeoJsonPointFeature() { Coordinates = new []{ double.Parse(update.VehicleLongitude), double.Parse(update.VehicleLatitude) }},
                Bearing = update.TravelDirection,
                Speed = update.TravelSpeed,
                IsCancelled = false
            };
        }

        public static Vehicle ToVehicle(this OdeBsmData bsm, Guid routeId, bool cancelled = false)
        {
            var bsmCore = bsm.Payload.Data.CoreData;
            var speed = new UnitOf.Speed().FromMetersPerSecond(bsmCore.Speed).ToMilesPerHour();
            var location = new GeoJsonPointFeature()
                {Coordinates = new[] {bsmCore.Position.Longitude, bsmCore.Position.Latitude}};
            return new Vehicle()
            {
                Timestamp = DateTime.Now,
                RouteId = routeId,
                Id = bsmCore.Id,
                Type = bsmCore.Size.ToVehicleType(),
                Name = bsmCore.Id,
                Location = location,
                Bearing = (int)bsm.Payload.Data.CoreData.Heading,
                Speed = speed,
                IsCancelled = cancelled
            };
        }

        public static VehicleUpdate ToVehicleUpdate(this OdeBsmData bsm, string? routeId = null, bool cancelled = false)
        {
            var bsmCore = bsm.Payload.Data.CoreData;
            var speed = new UnitOf.Speed().FromMetersPerSecond(bsmCore.Speed).ToMilesPerHour();
            
            return new VehicleUpdate()
            {
                RouteId = routeId,
                VehicleId = bsmCore.Id,
                VehicleType = bsmCore.Size.ToVehicleType(),
                VehicleName = bsmCore.Id,
                VehicleLatitude = bsmCore.Position.Latitude.ToString(),
                VehicleLongitude = bsmCore.Position.Longitude.ToString(),
                TravelDirection = (int)bsm.Payload.Data.CoreData.Heading,
                TravelSpeed = speed
            };
        }

        private static string ToVehicleType(this VehicleSize size)
        {
            var vehicleLength = new UnitOf.Length().FromCentimeters(size.Length).ToFeet();
            
            return vehicleLength switch
            {
                (> 34) and (< 40) => "firetruck",
                (>= 40) and (< 45) => "bus",
                >= 45 => "truck",
                _ => "car",
            };
        }

        public static VehicleUpdate ToVehicleUpdate(this BsmMessageContent bsm, string? routeId = null, bool cancelled = false)
        {
            var speedMetersPerSecond = bsm.Bsm.CoreData.Speed / 50;
            var speed = new UnitOf.Speed().FromMetersPerSecond(speedMetersPerSecond).ToMilesPerHour();
            
            return new VehicleUpdate()
            {
                RouteId = routeId,
                VehicleId = bsm.Bsm.CoreData.Id.ToString(),
                VehicleType = "bus",
                VehicleName = bsm.Bsm.CoreData.Id.ToString(),
                VehicleLatitude = bsm.Bsm.CoreData.Lat.ToDouble().ToString(),
                VehicleLongitude = bsm.Bsm.CoreData.Long.ToDouble().ToString(),
                TravelDirection = (int)bsm.Bsm.CoreData.Angle,
                TravelSpeed = speed
            };
        }

        public static VehicleUpdate ToVehicleUpdate(this SrmMessageContent srm, string? routeId = null, bool cancelled = false)
        {
            var speedMetersPerSecond = srm.Srm.Requestor.Position.Speed / 50;
            var speed = new UnitOf.Speed().FromMetersPerSecond(speedMetersPerSecond).ToMilesPerHour();
            
            return new VehicleUpdate()
            {
                RouteId = routeId,
                VehicleId = srm.Srm.Requestor.Id.VehicleId ?? srm.Srm.Requestor.Id.StationId.ToString() ?? srm.Srm.Requestor.Name,
                VehicleType = "bus",
                VehicleName = srm.Srm.Requestor.Name,
                VehicleLatitude = srm.Srm.Requestor.Position.Position.Latitude.ToDouble().ToString(),
                VehicleLongitude = srm.Srm.Requestor.Position.Position.Longitude.ToDouble().ToString(),
                TravelDirection = (int)srm.Srm.Requestor.Position.Heading,
                TravelSpeed = speed
            };
        }
        
        private static double ToDouble(this int value)
        {
            return (double)(value) / 10000000;
        }
    }
}
