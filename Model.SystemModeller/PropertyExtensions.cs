// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Runtime.CompilerServices;
using System.Text.Json;
using MongoDB.Bson;

namespace Econolite.Ode.Model.SystemModeller;

public static class PropertyExtensions
{
    private static Dictionary<string, Type> _fields = new Dictionary<string, Type>
    {
        {"intersection", typeof(string)},
        {"speed", typeof(int)},
        {"bearing", typeof(string)},
        {"phases", typeof(IEnumerable<PhaseModel>)}
    };
    
    public static BsonDocument ToBson(this IDictionary<string, dynamic> properties)
    {
        var doc = new BsonDocument();
        var elements = properties.Keys.Select(k => CreateBsonElement(k, properties[k])).Cast<BsonElement>();
        doc.AddRange(elements);
        return doc;
    }
    
    public static BsonDocument ToBson(this Dictionary<string, dynamic> properties)
    {
        return (properties as IDictionary<string, dynamic>).ToBson();
    }

    private static BsonElement CreateBsonElement(string key, dynamic value)
    {
        switch (value)
        {
            case IEnumerable<PhaseModel> models:
            {
                var list = models;
                return new BsonElement(key, list?.ToBsonArray() ?? new BsonArray());
            }
            case JsonElement element:
                if (key == "phases" && element.ValueKind == JsonValueKind.Array)
                {
                    var array = new BsonArray(element.ToJson());
                    return new BsonElement(key, array);
                }
                
                var result = JsonSerializer.Deserialize(element, key.ToType());
                if (result == null)
                {
                    return new BsonElement();
                }
                return new BsonElement(key, BsonValue.Create(result));
            default:
                return new BsonElement(key, value);
        }
    }

    public static BsonArray ToBsonArray(this IEnumerable<PhaseModel> model)
    {
        var result = new BsonArray();
        var phases = model.Select(m => m.ToBson());
        result.AddRange(phases);
        return result;
    }
    
    public static BsonDocument ToBson(this PhaseModel model)
    {
        // var doc = new BsonDocument()
        // {
        //     {"number", model.Number},
        //     {"movement", model.Movement},
        //     {"lanes", model.Lanes}
        // };
        // if (model.Detectors != null && model.Detectors.Any())
        // {
        //     doc.Add(new BsonElement("detectors",new BsonArray(model.Detectors.Select(d => d.ToBsonDocument()))));
        // }
        // return doc;
        return model.ToBsonDocument();
    }
    
    public static PhaseModel ToObject(this BsonDocument doc)
    {
        var elements = doc.Elements;
        return new PhaseModel() { Number = doc.GetValue("number").AsInt32, Lanes = doc.GetValue("lanes").AsInt32, Movement = doc.GetValue("number").AsString };
    }

    private static Type ToType(this string name)
    {
        return _fields[name];
    }
}
