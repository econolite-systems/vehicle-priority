// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Gtfs
{
    public class Trip
    {
        public string TripId { get; set; }

        public string RouteId { get; set; }

        public string ServiceId { get; set; }

        public string TripHeadsign { get; set; }

        public string TripShortName { get; set; }

        public string DirectionId { get; set; }

        public string BlockId { get; set; }

        public string ShapeId { get; set; }

        public bool WheelchairAccessible { get; set; }

        public bool BikesAllowed { get; set; }
    }
}
