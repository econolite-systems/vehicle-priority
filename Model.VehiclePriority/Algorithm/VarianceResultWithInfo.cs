// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    public class VarianceResultWithInfo
    {
        public VarianceResult VarianceResult { get; set; } = new VarianceResult();
        public ResultType ResultType { get; set; } = ResultType.Normal;
        public int MinSpeed { get; set; } = -1;
        public int MaxSpeed { get; set; } = -1;
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }
        public bool LinearCorrelation { get; set; } = false;
        public double Slope { get; set; }
        public double YIntercept { get; set; }
        public List<Tuple<int, int>> RemainingDistanceAndTime { get; set; }= new List<Tuple<int, int>>(); //Stores the average trip into the signal
    }
}
