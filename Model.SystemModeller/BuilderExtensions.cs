// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Buffer;

namespace Econolite.Ode.Domain.SystemModeller
{
    public static class BuilderExtensions
    {
        public static Polygon CreateBuffer(this LineString lineString, double distance) => BufferOp.Buffer((Geometry)lineString, distance) as Polygon ?? Polygon.Empty;
    }
}
