// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Model.SystemModeller;

public interface IGeoJsonType
{
    string GeoType { get; set; }
}

public interface IGeoJsonGeometry
{
    string Type { get; set; }
    double[][] Coordinates { get; set; }
}

public interface IGeoJsonGeoFence
{
    string Type { get; set; }
    
    double[][][] Coordinates { get; set; }
}
