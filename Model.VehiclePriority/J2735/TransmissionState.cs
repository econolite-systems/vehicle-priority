// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public enum TransmissionState
{
    NEUTRAL, // Neutral, speed relative to the vehicle alignment
    PARK, // Park, speed relative the to vehicle alignment
    FORWARDGEARS, // Forward gears, speed relative the to vehicle alignment
    REVERSEGEARS, // Reverse gears, speed relative the to vehicle alignment
    RESERVED1, RESERVED2, RESERVED3, UNAVAILABLE // not-equipped or unavailable
}
