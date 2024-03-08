// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Xunit;
// ReSharper disable HeapView.BoxingAllocation

namespace Econolite.Ode.Model.SystemModeller.Test;

public class IntersectionPropertyModelTests
{
    [Fact]
    public void IntersectionFeature_CreateWithProperties()
    {
    }
    
    [Fact]
    public void IntersectionFeature_CreateWithProperties_ToSerializableFeature()
    {
    }

    [Fact]
    public void IntersectionFeature_CreateWithConstructor()
    {
    }
    
    [Fact]
    public void IntersectionFeature_CreateWithConstructor_ToDto()
    {
        // var feature = new Feature
        // {
        //     Attributes = new AttributesTable()
        //     {
        //         {GeoJsonConverterFactory.DefaultIdPropertyName, Guid.NewGuid()},
        //         {"ingress", new [] { new ApproachFeature()
        //             {
        //                 Id = Guid.NewGuid(),
        //                 Bearing = Bearing.NB,
        //                 Geometry = ((IEnumerable<Coordinate>) new []
        //                 {
        //                     (-111.89123779535294, 40.760442523425205).ToCoordinate(),
        //                     (-111.89096823334694, 40.76043642853944).ToCoordinate(),
        //                 }).ToLineString(EpsgCode.Wgs84.ToInt()),
        //                 Phases = new[] { new Phase() },
        //                 Speed = 50
        //             }.ToDto(), 
        //             new ApproachFeature(){
        //                 Id = Guid.NewGuid(),
        //                 Bearing = Bearing.SB,
        //                 Geometry = ((IEnumerable<Coordinate>) new []
        //                 {
        //                     (-111.89125522971153, 40.76087830630891).ToCoordinate(),
        //                     (-111.89136251807211, 40.76078993099003).ToCoordinate(),
        //                 }).ToLineString(EpsgCode.Wgs84.ToInt()),
        //                 Phases = new[] { new Phase() },
        //                 Speed = 50
        //             }.ToDto()
        //         }},
        //     },
        //     Geometry = ((IEnumerable<Coordinate>) new []
        //     {
        //         (-111.89125522971153, 40.76087830630891).ToCoordinate(),
        //         (-111.89136251807211, 40.76078993099003).ToCoordinate(),
        //         (-111.8913584947586, 40.760521756889304).ToCoordinate(),
        //         (-111.89123779535294, 40.760442523425205).ToCoordinate(),
        //         (-111.89096823334694, 40.76043642853944).ToCoordinate(),
        //         (-111.89074829220772, 40.7605298833931).ToCoordinate(),
        //         (-111.89074963331223, 40.76079399422566).ToCoordinate(),
        //         (-111.89098566770554, 40.76087830630891).ToCoordinate(),
        //         (-111.89125522971153, 40.76087830630891).ToCoordinate()
        //     }).ToPolygon(EpsgCode.Wgs84.ToInt()),
        // };
        //
        // var intersection = new IntersectionFeature(feature);
        //
        // var dto = new IntersectionFeatureDto()
        // {
        //     Geometry = new GeoJsonGeoFence()
        //     {
        //         Coordinates = intersection.Geometry.Coordinates.ToPolygonDto()
        //     },
        //     Id = intersection.Id,
        //     Properties = intersection.Attributes.ToDto()
        // };
    }
}
