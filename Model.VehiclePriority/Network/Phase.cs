// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Network
{
    /// <summary>
    ///     Phase defines the movement of traffic thru the intersection.
    /// </summary>
    public class Phase
    {
        /// <summary>
        ///     Gets or sets the phase number validation of 1-16.
        /// </summary>
        /// <value>
        ///     The phase number.
        /// </value>
        public int Number { get; set; }

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
        ///     Gets or sets the number of lanes the phase covers.
        /// </summary>
        /// <value>
        ///     The number of lanes.
        /// </value>
        public int Lanes { get; set; }

        /// <summary>
        ///     Gets or sets the length.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public double Length { get; set; }

        /// <summary>
        ///     Gets or sets the speed limit in mph defined for the street.
        /// </summary>
        /// <value>
        ///     The speed limit mph.
        /// </value>
        public double SpeedLimit { get; set; }

        /// <summary>
        ///     Gets or sets the exit phase this is calculated from approach and movement.
        /// </summary>
        /// <value>
        ///     The exit phase.
        /// </value>
        public int? ExitPhase { get; set; }

        /// <summary>
        ///     Gets or sets the saturated flow in veh/hr for all lanes the phase covers.
        /// </summary>
        /// <value>
        ///     The saturated flow in veh/hr.
        /// </value>
        public double SaturatedFlow { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is channelized.
        /// </summary>
        /// <value>
        ///     The is channelized.
        /// </value>
        public bool IsChannelized { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is ped crossing.
        /// </summary>
        /// <value>
        ///     The is ped crossing.
        /// </value>
        public bool IsPedCrossing { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether it is right turn on red.
        /// </summary>
        /// <value>
        ///     The right turn on red.
        /// </value>
        public bool RightTurnOnRed { get; set; }

        /// <summary>
        ///     Gets or sets all the detectors associated with this phase.
        /// </summary>
        /// <value>
        ///     The all the detectors associated with this phase.
        /// </value>
        public IEnumerable<Detector> Detectors { get; set; } = Array.Empty<Detector>();
    }
}
