// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record FullPositionVector(
    OdePosition3d Position,
    double Heading,
    PositionalAccuracy PosAccuracy,
    PositionConfidenceSet PositionConfidenceSet,
    TransmissionAndSpeed Speed,
    SpeedAndHeadingAndThrottleConfidence SpeedConfidence,
    TimeConfidence TimeConfidence,
    DDateTime UtcTime
);
