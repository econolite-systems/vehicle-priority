// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Gtfs
{
    public class StopTime
    {
        public string TripId { get; set; }

        public string ArrivalTime { get; set; }

        public string DepartureTime { get; set; }

        public string StopId { get; set; }

        public int StopSequence { get; set; }

        public string StopHeadsign { get; set; }

        public string PickupType { get; set; }

        public string DropOffType { get; set; }

        public double ShapeDistTraveled { get; set; }

        public bool Timepoint { get; set; }
    }
}
