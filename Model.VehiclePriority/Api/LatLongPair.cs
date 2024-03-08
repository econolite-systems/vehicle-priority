// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Linq;

namespace Econolite.Ode.Models.VehiclePriority.Api;

public class LatLongPair
{
    public LatLongPair()
    {
    }

    public LatLongPair(double latitude, double longitude)
    {
        // validate range, this of course won't work across wcf since it uses default constructor.
        if (latitude > 90.0f || latitude < -90.0f)
            throw new ArgumentOutOfRangeException(nameof(latitude),
                $"Latitude argument should be between -90.0 and 90.0, it was {latitude}");

        if (longitude > 180.0f || longitude < -180.0f)
            throw new ArgumentOutOfRangeException(nameof(longitude),
                $"Latitude argument should be between -180.0 and 180.0, it was {Longitude}");

        Latitude = latitude;
        Longitude = longitude;
    }

    public double Latitude { get; }

    public double Longitude { get; }

    public override bool Equals(object? obj)
    {
        if (obj is LatLongPair arg)
            return Latitude.Equals(arg.Latitude) && Longitude.Equals(arg.Longitude);
        return false;
    }

    public override int GetHashCode()
    {
        return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Latitude}, {Longitude}";
    }
}

public static class LatLonPairExtensions
{
    public static double[][] ToDoubleArray(this LatLongPair[] pairs)
    {
        return pairs.Select(p => p.ToDoubleArray()).ToArray();
    }

    public static double[] ToDoubleArray(this LatLongPair pair)
    {
        return new [] {pair.Longitude, pair.Latitude};
    }
}
