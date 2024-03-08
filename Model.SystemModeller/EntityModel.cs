// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Ode.Persistence.Common.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Econolite.Ode.Model.SystemModeller;

public class EntityModel : GuidIndexedEntityBase
{
    public string EntityType { get; set; } = "Unknown";
    public JsonDocument? Geometry { get; set; }
    public GeoJsonPolygon? GeoFence { get; set; }
    public JsonDocument? Properties { get; set; }
    public bool? IsDeleted { get; set; }
}
