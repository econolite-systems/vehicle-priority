// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.J2735;

public record BsmCoreData (
    int MsgCnt,
    string Id,
    int SecMark,
    OdePosition3d Position,
    AccelerationSet4Way AccelSet,
    PositionalAccuracy Accuracy,
    TransmissionState Transmission,
    double Speed,
    double Heading,
    double Angle,
    BrakeSystemStatus Brakes,
    VehicleSize Size );
