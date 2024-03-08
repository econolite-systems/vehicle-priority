// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    public class TripGroup
    {
        public ResultType Type { get; set; } = ResultType.Normal;
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }

        public List<TripPoint> GroupedPoints { get; set; } = new List<TripPoint>();
    }
}
