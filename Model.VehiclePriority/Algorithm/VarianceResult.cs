// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    public class VarianceResult
    {
        public double Mean { get; set; }
        public double Deviation { get; set; } //sum of squared differences from mean
        public int Min  { get; set; }
        public int Max  { get; set; }
        public int ObsCount { get; set; }
    }
}
