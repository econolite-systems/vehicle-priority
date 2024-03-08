// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Models.VehiclePriority
{
    public class RouteStatus :  IIndexedEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public int RequestId { get; set; } 
        public int? DesiredClassLevel { get; set; }
        public string? VehicleId { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleName { get; set; }
        public int? VehicleTypePriority { get; set; }
        public Controller? PreviousIntersection { get; set; }
        public Controller? NextIntersection { get; set; }
        public TripPointLocation? Location { get; set; }
        public DateTime? Eta { get; set; }
        public int EtaInSeconds { get; set; }
        public Vehicle? Vehicle { get; set; }
        public GeoJsonLineStringFeature? Geometry { get; set; }
        public string? Metadata { get; set; }
        public string? Tag { get; set; }
        public bool IsInitial { get; set; }
        public DateTime? StartTime { get; set; }
        public bool Completed { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime LastUpdate { get; set; }
        
    }
}
