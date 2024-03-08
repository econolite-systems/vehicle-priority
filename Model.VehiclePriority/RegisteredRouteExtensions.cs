// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority.Api;

namespace Econolite.Ode.Models.VehiclePriority;

public static class RegisteredRouteExtensions
{
    public static RegisteredRoute ToRegisteredRoute(this RouteUpdate request)
    {
        return new RegisteredRoute
        {
            Id = Guid.Parse(request.RouteId),
            Destination = new GeoJsonPointFeature() { Coordinates = new []{ request.DestinationLongitude, request.DestinationLatitude } },
            DestinationCity = request.DestinationCity,
            DestinationLocation = request.DestinationLocation,
            Unit = new GeoJsonPointFeature() { Coordinates = new []{ request.UnitLongitude, request.UnitLatitude } },
            UnitCity = request.UnitCity,
            UnitLocation = request.UnitLocation,
            Geometry = new GeoJsonLineStringFeature() { Coordinates = request.Waypoints.ToDoubleArray() },
            LastUpdate = DateTime.Now
        };
    }
}
