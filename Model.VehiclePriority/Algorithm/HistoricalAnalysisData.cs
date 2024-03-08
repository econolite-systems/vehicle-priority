// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Algorithm;

public class HistoricalAnalysisData
{
    public HistoricalAnalysisData(VarianceResultWithInfo varianceData)
    {
        VarianceData = varianceData;
    }

    public class VarianceResultWithInfo
    {
        public VarianceResult VarianceResult { get; set; } = new VarianceResult();
        public ResultType ResultType { get; set; } = ResultType.Normal;
        public int MinSpeed = -1;
        public int MaxSpeed = -1; 
        public int MinDistance { get; set; }
        public int MaxDistance { get; set; }
        public bool LinearCorrelation { get; set; } = false;
        public double Slope { get; set; }
        public double YIntercept { get; set; }
        List<Tuple<int, int>> RemainingDistanceAndTime { get; set; } = new List<Tuple<int, int>>(); //Stores the average trip into the signal
    }

    public class VarianceResult
    {
        public double Mean { get; set; }
        public double Deviation { get; set; } //sum of squared differences from mean
        public int Min { get; set; }
        public int Max { get; set; }
        public int ObsCount { get; set; }
    }

    public VarianceResultWithInfo VarianceData { get; set; }
}
