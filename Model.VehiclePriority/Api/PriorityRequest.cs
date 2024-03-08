// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;

namespace Econolite.Ode.Models.VehiclePriority.Api
{
    /// <summary>
    ///     Data required for starting a priority request for a vehicle.
    /// </summary>
    public class PriorityRequest
    {
        /// <summary>
        ///     ID to use to identify this request in the system.
        /// </summary>
        public string RequestId { get; set; } = String.Empty;

        /// <summary>
        ///     ID of the vehicle from remote system.
        /// </summary>
        public string VehicleId { get; set; } = String.Empty;

        /// <summary>
        ///     Type of vehicle. firetruck, bus, snowplow, freight, passenger car.
        /// </summary>
        public string VehicleType { get; set; } = String.Empty;

        /// <summary>
        ///     Name of vehicle from remote system.
        /// </summary>
        public string VehicleName { get; set; } = String.Empty;

        /// <summary>
        ///     Any external data, such as an ID, that the user wishes to track with the request.
        /// </summary>
        public string Tag { get; set; } = String.Empty;

        /// <summary>
        ///     Supplemental data to be processed with the request.
        /// </summary>
        public string RequestMetadata { get; set; } = String.Empty;

        /// <summary>
        ///     ID of the route the vehicle is on if known, null if not known.  This
        ///     should be populated from the RegisterRoute API.
        /// </summary>
        public string RouteId { get; set; } = String.Empty;

        /// <summary>
        ///     Class 1-10 priority.  This can be overriden by user configuraiton in Centracs.
        /// </summary>
        public int? DesiredClassLevel { get; set; }
    }
}
