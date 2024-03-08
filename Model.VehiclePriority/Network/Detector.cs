// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     The Detector.
    /// </summary>
    public class Detector
    {
        /// <summary>
        ///     Gets or sets the detector number validation of 1-64.
        /// </summary>
        /// <value>
        ///     The detector number.
        /// </value>
        public int Number { get; set; }

        /// <summary>
        ///     Gets or sets the detector name - copied from the Mongo db store. In the format of: "detector number - laneType -
        ///     approach".
        /// </summary>
        /// <value>
        ///     The detector name.
        /// </value>
        public string DetectorName { get; set; } = String.Empty;

        /// <summary>
        ///     Gets or sets the bearing being north, east, south, west.
        /// </summary>
        /// <value>
        ///     The bearing.
        /// </value>
        public Bearing Bearing { get; set; }

        /// <summary>
        ///     Gets or sets the movement being left, thru, right.
        /// </summary>
        /// <value>
        ///     The heading.
        /// </value>
        public Movement Movement { get; set; }

        /// <summary>
        ///     Gets or sets the detector length.
        /// </summary>
        /// <value>
        ///     The detector length.
        /// </value>
        public int Length { get; set; }

        /// <summary>
        ///     Gets or sets the set back from the stop bar.
        /// </summary>
        /// <value>
        ///     The set back in feet from stop bar.
        /// </value>
        public int SetBack { get; set; }

        /// <summary>
        ///     Gets or sets the distance to the next signals stop bar.
        /// </summary>
        /// <value>
        ///     The distance to the next signals stop bar.
        /// </value>
        public double DistanceToNextSignal { get; set; }

        /// <summary>
        ///     Gets or sets the number of lanes the detector covers.
        /// </summary>
        /// <value>
        ///     The number of lanes.
        /// </value>
        public int Lanes { get; set; }

        /// <summary>
        ///     Gets or sets the number of lanes the detector covers.
        /// </summary>
        /// <value>
        ///     The number of lanes.
        /// </value>
        public int LaneNumber { get; set; }

        /// <summary>
        ///     Gets or sets all the phases this detector covers.
        /// </summary>
        /// <value>
        ///     The all phases.
        /// </value>
        public IEnumerable<int> Phases { get; set; } = Array.Empty<int>();

        /// <summary>
        ///     Gets or sets the protected phase.
        /// </summary>
        /// <value>
        ///     The protected phase.
        /// </value>
        public int ProtectedPhase { get; set; }

        /// <summary>
        ///     Gets or sets the permitted phase.
        /// </summary>
        /// <value>
        ///     The permitted phase.
        /// </value>
        public int PermittedPhase { get; set; }

        /// <summary>
        ///     Gets or sets the detector type this is informational.
        /// </summary>
        /// <value>
        ///     The detector type.
        /// </value>
        public DetectorType Type { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is advanced.
        /// </summary>
        /// <value>
        ///     The advanced.
        /// </value>
        public bool Advanced { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is phase data.
        /// </summary>
        /// <value>
        ///     The phase data.
        /// </value>
        public bool PhaseData { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is exit detection.
        /// </summary>
        /// <value>
        ///     The exit detection.
        /// </value>
        public bool ExitDetection { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is turning movement count.
        /// </summary>
        /// <value>
        ///     The turning movement count.
        /// </value>
        public bool TurningMovementCount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is split failure.
        /// </summary>
        /// <value>
        ///     The split failure.
        /// </value>
        public bool SplitFailure { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is red light monitor.
        /// </summary>
        /// <value>
        ///     The red light monitor.
        /// </value>
        public bool RedLightMonitor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it provides speed.
        /// </summary>
        /// <value>
        ///     The provides speed.
        /// </value>
        public bool Speed { get; set; }
    }
}
