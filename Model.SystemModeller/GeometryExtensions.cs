// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using System.Collections.Generic;
using Econolite.Ode.Model.SystemModeller;

namespace Econolite.Ode.Domain.SystemModeller
{
  public static class GeometryExtensions
  {
    private static readonly CoordinateSystemServices Services = new CoordinateSystemServices(
      (IEnumerable<KeyValuePair<int, string>>) new Dictionary<int, string>()
      {
        {
          4326,
          ((Info) GeographicCoordinateSystem.WGS84).WKT
        },
        {
          3857,
          ((Info) ProjectedCoordinateSystem.WebMercator).WKT
        }
      });

    public static Geometry ProjectTo(this Geometry geometry, EpsgCode toSrid) => geometry.Coordinates.Length == 1
      ? geometry.ProjectToSingle(toSrid)
      : geometry.ProjectToMany(toSrid);

    private static Geometry ProjectToSingle(this Geometry geometry, EpsgCode toSrid)
    {
      MathTransformFilter mathTransformFilter = new MathTransformFilter(GeometryExtensions.Services
        .CreateTransformation(geometry.SRID, (int) toSrid).MathTransform);
      Geometry single = geometry.Copy();
      single.Apply((ICoordinateSequenceFilter) mathTransformFilter);
      single.SRID = (int) toSrid;
      return single;
    }

    private static Geometry ProjectToMany(this Geometry geometry, EpsgCode toSrid)
    {
      int srid = geometry.SRID;
      Geometry many = geometry.Copy();
      for (int index = 0; index < many.Coordinates.Length; ++index)
      {
        Coordinate coordinate = many.Coordinates[index].ProjectTo(srid.ToEpsgCode(), toSrid);
        many.Coordinates[index].CoordinateValue = coordinate.CoordinateValue;
      }

      many.SRID = toSrid.ToInt();
      return many;
    }

    public static GeoJsonPolygon ToGeoJsonPolygon(this Polygon polygon)
    {
      return new GeoJsonPolygon()
      {
        Coordinates = polygon.Coordinates.ToGeoJsonPolygonCoordinates()
      };
    }
    
    public static Polygon ToPolygon(this GeoJsonPolygon polygon)
    {
      // return new Polygon(
      //   new LinearRing(polygon.Coordinates.ToCoordinates())
      // );
      return Geometry.DefaultFactory.CreatePolygon(polygon.Coordinates.ToCoordinates());
    }
    
    public static LineString ToLinestring(this GeoJsonLineString json, bool reverseCoordinates = false)
    {
      if (json.Type != "LineString")
      {
        throw new ArgumentException($"Invalid Type {json.Type} should be LineString");
      }

      var coordinates = reverseCoordinates
        ? json.Coordinates.ToCoordinates().Reverse()
        : json.Coordinates.ToCoordinates();
      return new LineString(coordinates.ToArray());
    }

    private static Coordinate[] ToCoordinates(this double[][][] coordinates)
    {
      return coordinates.Select(v => v.ToCoordinates()).SelectMany(c => c).ToArray();
    }
    
    private static Coordinate[] ToCoordinates(this double[][] coordinates)
    {
      return coordinates.Select(v => v.ToCoordinate()).ToArray();
    }

    private static Coordinate ToCoordinate(this double[] coordinate)
    {
      ArgumentNullException.ThrowIfNull(coordinate);
      if (coordinate.Length < 2)
      {
        throw new ArgumentException($"Wrong length for a coordinate {coordinate.Length} should be at least 2");
      }

      return new Coordinate(coordinate[0], coordinate[1]);
    }

    private static double[][][] ToGeoJsonPolygonCoordinates(this Coordinate[] coordinates)
    {
      var result = coordinates.Select(c => new[] {c.X, c.Y}).ToArray();
      return new[] {result};
    }
  }
}
