// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;

namespace Econolite.Ode.Model.SystemModeller;

public abstract class GeoJsonGeometryBase : IGeoJsonGeometry
{
    public abstract string Type { get; set; }
    public double[][] Coordinates { get; set; } = Array.Empty<double[]>();
}

public class GeoJsonGeometry : GeoJsonGeometryBase
{
    public override string Type { get; set; } = String.Empty;
}

public class GeoJsonPoint
{
    public string? Type { get; set; } = "Point";
    public double[] Coordinates { get; set; } = Array.Empty<double>();
}

public class GeoJsonLineString
{
    public string? Type { get; set; } = "LineString";
    public double[][] Coordinates { get; set; } = Array.Empty<double[]>();
}

public class GeoJsonPolygon
{
    public string Type { get; set; } = "Polygon";
    public double[][][] Coordinates { get; set; } = Array.Empty<double[][]>();
}

public static class GeoJsonExtensions
{
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    public static GeoJsonLineString ToLineString(this JsonDocument json)
    {
        return JsonSerializer.Deserialize<GeoJsonLineString>(json, _jsonOptions) ?? throw new InvalidOperationException();
    }
    
    public static GeoJsonPoint ToPoint(this JsonDocument json)
    {
        return JsonSerializer.Deserialize<GeoJsonPoint>(json, _jsonOptions) ?? throw new InvalidOperationException();
    }
}
