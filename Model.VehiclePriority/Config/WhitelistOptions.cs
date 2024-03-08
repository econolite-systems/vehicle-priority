// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Config;

public class WhitelistOptions
{
    public const string Section = "Whitelist";

    public string[] Vehicles { get; set; } = Array.Empty<string>();
}
