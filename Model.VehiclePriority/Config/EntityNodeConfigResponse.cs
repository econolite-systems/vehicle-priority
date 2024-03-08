// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using Econolite.Ode.Models.Entities.Spatial;

namespace Econolite.Ode.Models.VehiclePriority.Config;

public record EntityNodeJsonConfigResponse(string Json) : GenericJsonResponse(Json);

public class EntityNodeConfigResponse
{
    public IEnumerable<EntityModel> Result { get; set; } = Array.Empty<EntityModel>();
}

