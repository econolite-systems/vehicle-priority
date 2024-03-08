// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using NetTopologySuite.Geometries;

namespace Econolite.Ode.Domain.SystemModeller
{
  public static class CoordinateExtensions
  {
    public static Coordinate ProjectTo(
      this Coordinate coordinate,
      EpsgCode fromSrid,
      EpsgCode toSrid)
    {
      Point point = new Point(coordinate);
      ((Geometry) point).SRID = fromSrid.ToInt();
      return ((Geometry) point).ProjectTo(toSrid).Coordinate;
    }
  }
}
