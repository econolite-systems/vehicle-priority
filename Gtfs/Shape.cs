// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Gtfs
{
    public class Shape
    {
        public string ShapeId { get; set; }

        public double ShapePtLat { get; set; }

        public double ShapePtLon { get; set; }

        public int ShapePtSequence { get; set; }

        public double ShapeDistTraveled { get; set; }
    }
}
