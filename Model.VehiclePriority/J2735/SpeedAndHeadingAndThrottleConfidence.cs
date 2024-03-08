// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record SpeedAndHeadingAndThrottleConfidence (
    HeadingConfidence Heading,
    SpeedConfidence Speed,
    ThrottleConfidence Throttle
    );
