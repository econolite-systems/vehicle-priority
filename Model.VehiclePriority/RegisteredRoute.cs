// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Persistence.Common.Interfaces;

namespace Econolite.Ode.Models.VehiclePriority;

public class RegisteredRoute : IIndexedEntity<Guid>
{
    public Guid Id { get; set; }
    
    public string? DestinationLocation { get; set; }
    
    public string? DestinationCity { get; set; }

    public GeoJsonPointFeature? Destination { get; set; }
    
    public string? UnitLocation { get; set; }
    
    public string? UnitCity { get; set; }
    
    public GeoJsonPointFeature? Unit { get; set; }
    public GeoJsonLineStringFeature? Geometry { get; set; }
    
    public IEnumerable<Controller> Intersections { get; set; } = Array.Empty<Controller>();

    public DateTime LastUpdate { get; set; }
}
