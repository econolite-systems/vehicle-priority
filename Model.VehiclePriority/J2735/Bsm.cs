// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record Bsm(
    BsmCoreData CoreData,
    IEnumerable<BsmPart2Content> PartII);

public record BsmPart2Content(string Id, VehicleSafetyExtensions Value);
