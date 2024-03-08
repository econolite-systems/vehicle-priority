// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Runtime.InteropServices;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct RawBsm
{
    public byte RxSource;
    public RawBsmLocation Location;
    public int Time;
    public short mSec;
    public byte SecurityResultCode;
    public short PayloadLength;
}

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct RawBsmLocation
{
    public int Latitude;
    public int Longitude;
    public int Elevation;
    public short Speed;
    public short Heading;
}
