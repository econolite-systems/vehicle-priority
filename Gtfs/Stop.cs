// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Gtfs
{
    public class Stop
    {
        public string StopId { get; set; }

        public string StopCode { get; set; }

        public string StopName { get; set; }

        public string StopDesc { get; set; }

        public double StopLat { get; set; }

        public double StopLon { get; set; }

        public string ZoneId { get; set; }

        public string StopUrl { get; set; }

        public string LocationType { get; set; }

        public string ParentStation { get; set; }

        public string StopTimezone { get; set; }

        public int WheelchairBoarding { get; set; }
    }
}
