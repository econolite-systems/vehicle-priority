// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record PathHistory (
    FullPositionVector InitialPosition,
    Dictionary<string, bool> CurrGNSSstatus,
    IEnumerable<PathHistoryPoint> CrumbData);
