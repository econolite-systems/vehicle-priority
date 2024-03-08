// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    public class TripPointData
    {
        public IEnumerable<TripPoint> TripPoints { get; set; } = new List<TripPoint>();
        public string ApproachId { get; set; } = String.Empty;
        public string RouteId { get; set; } = String.Empty;
    }
}
