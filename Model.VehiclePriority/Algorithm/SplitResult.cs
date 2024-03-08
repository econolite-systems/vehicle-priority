// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm
{
    public class SplitResult
    {
        public VarianceResult LowerVariance { get; set; } = new VarianceResult();
        public VarianceResult UpperVariance { get; set; } = new VarianceResult();
        public int SplitSpeed { get; set; } //This value and lower goes to lowerVariance, higher than this value goes to upperVariance
    }
}
