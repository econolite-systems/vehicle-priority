// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public enum PositionConfidence
{
    UNAVAILABLE,
    A500M,
    A200M,
    A100M,
    A50M,
    A20M,
    A10M,
    A5M,
    A2M,
    A1M,
    A50CM,
    A20CM,
    A10CM,
    A5CM,
    A2CM,
    A1CM
}

public enum ElevationConfidence {
    UNAVAILABLE,
    ELEV_500_00,
    ELEV_200_00,
    ELEV_100_00,
    ELEV_050_00,
    ELEV_020_00,
    ELEV_010_00,
    ELEV_005_00,
    ELEV_002_00,
    ELEV_001_00,
    ELEV_000_50,
    ELEV_000_20,
    ELEV_000_10,
    ELEV_000_05,
    ELEV_000_02,
    ELEV_000_01
}

public record PositionConfidenceSet(
    PositionConfidence Pos,
    ElevationConfidence Elevation );
