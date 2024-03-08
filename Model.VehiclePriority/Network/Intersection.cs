// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     The Intersection.
    /// </summary>
    public class Intersection : Unique<Guid>
    {
        /// <summary>
        ///     Gets or sets the Controller Type.
        /// </summary>
        /// <value>
        ///     The Controller Type.
        /// </value>
        public string ControllerType { get; set; } = String.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the idm configuration is valid.
        /// </summary>
        /// <value>
        ///     The is valid.
        /// </value>
        public bool IsValid { get; set; }

        /// <summary>
        ///     Gets or sets is idm configuration errors.
        /// </summary>
        /// <value>
        ///     The configuration errors.
        /// </value>
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();

        /// <summary>
        ///     Gets or sets the location point for the intersection.
        /// </summary>
        /// <value>
        ///     The location point.
        /// </value>
        public Point Location { get; set; } = Point.Empty;

        /// <summary>
        ///     Gets or sets the geofence for the intersection.
        /// </summary>
        /// <value>
        ///     The polygon that is used for the GeoFence.
        /// </value>
        public Polygon GeoFence { get; set; } = Polygon.Empty;

        /// <summary>
        ///     Gets or sets the ingresses into the intersection.
        /// </summary>
        /// <value>
        ///     The ingress of approaches.
        /// </value>
        public IEnumerable<Approach> Ingress { get; set; } = Array.Empty<Approach>();
    }
}
