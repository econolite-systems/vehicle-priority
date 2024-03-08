// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;

namespace Econolite.Ode.Model.SystemModeller;

public class ApproachTypeModel
{
    public string Name { get; set; } = String.Empty;

    public JsonDocument? Properties { get; set; }

}
