// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json.Serialization;
using Econolite.Ode.Model.SystemModeller.Db;
using MongoDB.Bson.Serialization.Attributes;

namespace Econolite.Ode.Model.SystemModeller
{
  public class PhaseModel
  {
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [BsonIgnoreIfNull]
    public int? Number { get; set; } = 2;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [BsonIgnoreIfNull]
    public string? Movement { get; set; } = "Thru";
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [BsonIgnoreIfNull]
    public int? Lanes { get; set; } = 1;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [BsonIgnoreIfNull]
    public IEnumerable<DetectorModel>? Detectors { get; set; } = Array.Empty<DetectorModel>();
  }
}
