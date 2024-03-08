// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    // public class EtaResult
    // {
    //     public List<VarianceResultWithInfo> ResultsList { get; set; } = new List<VarianceResultWithInfo>();
    //     //TODO: Update if necessary
    //     public int ApproachId { get; set; }
    //     public int RouteId { get; set; }
    // }
    
    public class EtaResult
    {
        public int Eta { get; set; }
        public float Variance { get; set; }
        public bool VarianceTooHigh { get; set; } //If true, sending ETA is not recommended due to large variance
    }
}
