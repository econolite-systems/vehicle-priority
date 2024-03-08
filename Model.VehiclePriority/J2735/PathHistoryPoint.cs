// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record PathHistoryPoint (
    double ElevationOffset,
    double Heading,
    double LatOffset,
    double LonOffset,
    PositionalAccuracy PosAccuracy,
    double Speed,
    double TimeOffset
);
