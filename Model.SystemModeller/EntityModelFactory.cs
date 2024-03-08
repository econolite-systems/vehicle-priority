// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Model.SystemModeller;

namespace Econolite.Ode.Domain.SystemModeller;

public static class EntityModelFactory
{
    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    public static EntityModel Create(EntityModel model)
    {
        var jsonOptions = JsonOptions;
        
        switch (model.EntityType)
        {
            case "StreetSegment":
                // Calculate the TripPoint Locations;
                if (model.Geometry != null)
                {
                    var geometry = model.Geometry.Deserialize<GeoJsonLineString>(jsonOptions);
                    if (model.Properties != null)
                    {
                        var properties = model.Properties.Deserialize<StreetSegmentPropertiesModel>(jsonOptions);
                        if (geometry!= null)
                            if (properties != null)
                            {
                                properties.TripPointLocations = geometry.ToTripPointLocations(50).ToArray();
                                model.Properties = JsonSerializer.SerializeToDocument(properties, jsonOptions);
                            }
                    }

                    if (geometry != null) model.GeoFence = geometry.ToLinestring().CreateBuffer(0.0001).ToGeoJsonPolygon();
                }

                return model;
            case "Approach":
                if (model.Properties != null)
                {
                    var approachProperties = model.Properties.Deserialize<ApproachPropertiesModel>(jsonOptions);
                    if (approachProperties != null)
                    {
                        model.Properties = JsonSerializer.SerializeToDocument(approachProperties, jsonOptions);
                    }
                }

                return model;
            case "Intersection":
                if (model.Properties != null)
                {
                    var intersectionProperties = model.Properties.Deserialize<IntersectionPropertiesModel>(jsonOptions);
                    if (intersectionProperties != null)
                    {
                        intersectionProperties.Intersection = model.Id.ToString();
                        model.Properties = JsonSerializer.SerializeToDocument(intersectionProperties, jsonOptions);
                    }
                }

                return model;
            default:
                return model;
        }
    }

    public static StreetSegmentPropertiesModel? ToStreetSegmentPropertiesModel(this EntityModel model)
    {
        if (model.Properties == null)
        {
            return null;
        }
        
        return model.Properties.Deserialize<StreetSegmentPropertiesModel>(JsonOptions);
    }
    
    public static ApproachPropertiesModel? ToApproachPropertiesModel(this EntityModel model)
    {
        if (model.Properties == null)
        {
            return null;
        }
        
        return model.Properties.Deserialize<ApproachPropertiesModel>(JsonOptions);
    }
    
    public static IntersectionPropertiesModel? ToIntersectionPropertiesModel(this EntityModel model)
    {
        if (model.Properties == null)
        {
            return null;
        }
        
        return model.Properties.Deserialize<IntersectionPropertiesModel>(JsonOptions);
    }
}
