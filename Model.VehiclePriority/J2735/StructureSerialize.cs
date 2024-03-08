// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.IO;
using System.Runtime.InteropServices;

namespace Econolite.Ode.Models.VehiclePriority.J2735;

public static class StructureSerialize
{
    public static T ReadStruct<T>(Stream stream)
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        _ = stream.Read(buffer);

        return ReadStruct<T>(buffer);
    }

    public static T ReadStruct<T>(byte[] buffer)
    {
        T result = default(T)!;
        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        try
        {
#pragma warning disable CS8600
            result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
#pragma warning restore CS8600
        }
        finally
        {
            handle.Free();
        }
#pragma warning disable CS8603
        return result;
#pragma warning restore CS8603
    }

    public static byte[] RawSerialize(object anything)
    {
        byte[] rawdata = new byte[Marshal.SizeOf(anything)];
        GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
        try
        {
            Marshal.StructureToPtr(anything, handle.AddrOfPinnedObject(), false);
        }
        finally
        {
            handle.Free();
        }
        return rawdata;
    }
    
    
}
