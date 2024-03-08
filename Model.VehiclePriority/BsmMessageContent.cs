// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using Econolite.Ode.Models.VehiclePriority.J2735;

namespace Econolite.Ode.Models.VehiclePriority;

public record BsmMessage(BsmMessageContent[] BsmMessageContent);

public record BsmMessageContent(BsmMessageMetadata Metadata, BsmMessagePayload Bsm);

public record BsmMessageMetadata(DateTime Utctimestamp);

public record BsmMessagePayload(BsmCoreMessagePayload CoreData);

public record BsmCoreMessagePayload(
    int MsgCnt,
    int Id,
    int SecMark,
    int Lat,
    int Long,
    int Elev,
    PositionalAccuracy Accuracy,
    TransmissionState Transmission,
    double Speed,
    double Heading,
    double Angle,
    BrakeSystemStatusMessagePayload Brakes,
    VehicleSize Size );

public record BrakeSystemStatusMessagePayload(
    int WheelBrakes,
    int Traction,
    int Abs,
    int Scs, 
    int BrakeBoost,
    int AuxBrakes );
