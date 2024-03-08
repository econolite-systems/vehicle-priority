// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     Detector types.
    /// </summary>
    public enum DetectorType
    {
        /// <summary>
        ///     Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        ///     InductiveLoop.
        /// </summary>
        InductiveLoop,

        /// <summary>
        ///     Electrometric.
        /// </summary>
        Electrometric,

        /// <summary>
        ///     Video.
        /// </summary>
        Video,

        /// <summary>
        ///     RadarFrontal.
        /// </summary>
        RadarFrontal,

        /// <summary>
        ///     RadarSideFire.
        /// </summary>
        RadarSideFire,

        /// <summary>
        ///     Hybrid.
        /// </summary>
        Hybrid
    }
}
