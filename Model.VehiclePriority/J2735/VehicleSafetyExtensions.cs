// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record VehicleSafetyExtensions(
    Dictionary<string, bool> Events,
    PathHistory PathHistory,
    PathPrediction PathPrediction,
    Dictionary<string, bool> Lights
    );
