// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority;

public record OdeLogMsgMetadataLocation(
    string Latitude,
    string Longitude,
    string Elevation,
    string Speed,
    string Heading);
