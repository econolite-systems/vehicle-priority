// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record DDateTime(
    int Day,
    int Hour,
    int Minute,
    int Month,
    int Offset,
    int Second,
    int Year
);
