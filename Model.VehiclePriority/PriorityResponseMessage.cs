// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;
using Econolite.Ode.Models.VehiclePriority.Config;

namespace Econolite.Ode.Models.VehiclePriority;

public record PriorityResponseConfigurationMessage(string Json) : GenericJsonResponse(Json);

public record PriorityResponseMessage(PriorityResponse PriorityResponse);

public record PriorityResponse(
    int PriorityRequestStatus,
    int RequestId,
    int StrategyNumber,
    int VehicleClassType,
    int VehicleClassLevel,
    string VehicleId
    );
