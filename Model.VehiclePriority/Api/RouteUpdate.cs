// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Models.VehiclePriority.Api
{
    public class RouteUpdate
    {
        /// <summary>
        ///     ID of the route provided by RequestPriority API for updating with UpdateRoute.  Empty if it is new.
        /// </summary>
        public string RouteId { get; set; } = String.Empty;

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public string DestinationLocation { get; set; } = String.Empty;

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public string DestinationCity { get; set; } = String.Empty;

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public double DestinationLatitude { get; set; }

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public double DestinationLongitude { get; set; }

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public string UnitLocation { get; set; } = String.Empty;

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public string UnitCity { get; set; } = String.Empty;

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public double UnitLatitude { get; set; }

        /// <summary>
        ///     Metadata to show in the system for user feedback.  Leave empty if not applicable.
        /// </summary>
        public double UnitLongitude { get; set; }

        public LatLongPair[] Waypoints { get; set; } = Array.Empty<LatLongPair>();
    }
}
