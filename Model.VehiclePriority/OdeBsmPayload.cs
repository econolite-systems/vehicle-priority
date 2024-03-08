// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.J2735;

namespace Econolite.Ode.Models.VehiclePriority;

public record OdeBsmPayload(string DataType, Bsm Data);
