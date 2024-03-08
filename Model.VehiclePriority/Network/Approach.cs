// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     The Approach or ingress into the intersection.
    /// </summary>
    public class Approach
    {
        /// <summary>
        ///     Gets or sets the bearing being north, east, south, west.
        /// </summary>
        /// <value>
        ///     The approach.
        /// </value>
        public Bearing Bearing { get; set; }

        /// <summary>
        ///     Gets or sets the speed limit in mph defined for the street.
        /// </summary>
        /// <value>
        ///     The speed limit mph.
        /// </value>
        public double SpeedLimit { get; set; }

        /// <summary>
        ///     Gets or sets the geometry for the Approach with at least two coordinates
        ///     one being the intersection.
        /// </summary>
        /// <value>
        ///     The speed limit mph.
        /// </value>
        public LineString Geometry { get; set; } = LineString.Empty;

        /// <summary>
        ///     Gets or sets all the phases on this approach.
        /// </summary>
        /// <value>
        ///     All phases for the approach.
        /// </value>
        public IEnumerable<Phase> Phases { get; set; } = Array.Empty<Phase>();

        /// <summary>
        ///     Gets or sets all the detectors on this approach.
        /// </summary>
        /// <value>
        ///     All detectors for the approach.
        /// </value>
        public IEnumerable<Detector> Detectors { get; set; } = Array.Empty<Detector>();
    }
}
