// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using NetTopologySuite.Geometries;

namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    /// </summary>
    public class StreetSegment : Unique<Guid>
    {
        /// <summary>
        ///     The intersection that is the ingress to the street segment.
        /// </summary>
        /// <value>
        ///     Source <see cref="Intersection" />.
        /// </value>
        public Intersection Source { get; set; } = new Intersection();

        /// <summary>
        ///     The intersection that is the egress of the street segment.
        /// </summary>
        /// <value>
        ///     Destination <see cref="Intersection" />
        /// </value>
        public Intersection Destination { get; set; } = new Intersection();

        /// <summary>
        ///     Is the street segment a one way?
        /// </summary>
        /// <value>
        ///     If this will be true if it's one way
        /// </value>
        public bool IsOneWay { get; set; }

        /// <summary>
        ///     The centerline of the street segment.
        /// </summary>
        /// <value>
        ///     A <see cref="LineString" /> for the street segment.
        /// </value>
        public LineString Geometry { get; set; } = LineString.Empty;

        /// <summary>
        ///     GeoFence is a buffer around the geometry.
        /// </summary>
        /// <value>
        ///     A <see cref="Polygon" /> for the street segment.
        /// </value>
        public Polygon GeoFence { get; set; } = Polygon.Empty;
    }
}
