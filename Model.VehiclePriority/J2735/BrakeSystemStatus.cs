// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record BrakeSystemStatus(
    Dictionary<string, bool> WheelBrakes,
    string Traction,
    string Abs,
    string Scs, 
    string BrakeBoost,
    string AuxBrakes );
