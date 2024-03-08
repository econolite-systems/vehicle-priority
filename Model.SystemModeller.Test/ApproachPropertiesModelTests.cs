// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Xunit;
// ReSharper disable HeapView.BoxingAllocation

namespace Econolite.Ode.Model.SystemModeller.Test;

public class UnitTest1
{
    // [Fact]
    // public void ApproachFeature_CreateWithProperties()
    // {
    //     var approach = new ApproachFeature
    //     {
    //         Id = Guid.NewGuid(),
    //         Bearing = Bearing.NB,
    //         Geometry = ((IEnumerable<Coordinate>) new Coordinate[4]
    //         {
    //             (-84.190029501915, 34.017980435211).ToCoordinate(),
    //             (-84.1900938749313, 34.0177892414078).ToCoordinate(),
    //             (-84.190571308136, 34.0163397115892).ToCoordinate(),
    //             (-84.1910004615784, 34.0149657479003).ToCoordinate()
    //         }).ToLineString(EpsgCode.Wgs84.ToInt()),
    //         Phases = new[] { new Phase() },
    //         Speed = 50
    //     };
    // }
    //
    // [Fact]
    // public void ApproachFeature_CreateWithProperties_ToSerializableFeature()
    // {
    //     var approach = new ApproachFeature
    //     {
    //         Id = Guid.NewGuid(),
    //         Bearing = Bearing.NB,
    //         Geometry = ((IEnumerable<Coordinate>) new Coordinate[4]
    //         {
    //             (-84.190029501915, 34.017980435211).ToCoordinate(),
    //             (-84.1900938749313, 34.0177892414078).ToCoordinate(),
    //             (-84.190571308136, 34.0163397115892).ToCoordinate(),
    //             (-84.1910004615784, 34.0149657479003).ToCoordinate()
    //         }).ToLineString(EpsgCode.Wgs84.ToInt()),
    //         Phases = new[] { new Phase() },
    //         Speed = 50
    //     };
    //
    //     var feature = approach.ToSerializableFeature();
    // }
    //
    // [Fact]
    // public void ApproachFeature_CreateWithConstructor()
    // {
    //     var feature = new Feature
    //     {
    //         Attributes = new AttributesTable()
    //         {
    //             {GeoJsonConverterFactory.DefaultIdPropertyName, Guid.NewGuid()},
    //             {"bearing", Bearing.NB},
    //             {"egress", Guid.Empty},
    //             {"phases", new [] { new Phase() }},
    //             {"speed", 50}
    //         },
    //         Geometry = ((IEnumerable<Coordinate>) new Coordinate[4]
    //         {
    //             (-84.190029501915, 34.017980435211).ToCoordinate(),
    //             (-84.1900938749313, 34.0177892414078).ToCoordinate(),
    //             (-84.190571308136, 34.0163397115892).ToCoordinate(),
    //             (-84.1910004615784, 34.0149657479003).ToCoordinate()
    //         }).ToLineString(EpsgCode.Wgs84.ToInt()),
    //     };
    //
    //     var approach = new ApproachFeature(feature);
    // }
    //
    // [Fact]
    // public void Properties_ToBson()
    // {
    //     //var phases = new BsonArray();
    //     
    //     var properties = new Dictionary<string, dynamic>()
    //     {
    //         {"intersection", Guid.NewGuid()},
    //         {"bearing", Bearing.NB.ToString()},
    //         {
    //             "phases",
    //             new []
    //             {
    //                 new PhaseModel() {Number = 2, Movement = Movement.Thru.ToString(), Lanes = 2}.ToBson(),
    //                 new PhaseModel() {Number = 1, Movement = Movement.Left.ToString(), Lanes = 1}.ToBson()
    //             }
    //         },
    //         {"speed", 35},
    //     };
    //
    //     var bson = properties.ToBson();
    // }
}
