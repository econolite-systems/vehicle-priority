// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Econolite.Ode.Model.SystemModeller;

public class IntersectionPropertiesModel
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [BsonIgnoreIfNull]
    public string? Intersection { get; set; }
}
