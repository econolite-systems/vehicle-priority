// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Econolite.OdeRepository.SystemModeller;

public class EntityNodeEdgeRepository : IEntityNodeJsonFileRepository
{
    private readonly string _path = "./data/config.json";
    private DateTime _lastLoad = DateTime.MinValue;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<EntityNodeEdgeRepository> _logger;

    public EntityNodeEdgeRepository(IMongoContext context, IMemoryCache memoryCache, ILogger<EntityNodeEdgeRepository> logger)
    {
        DbContext = context;
        _memoryCache = memoryCache;
        _logger = logger;
        CollectionName = "SystemModeller";
    }
    
    public EntityNodeEdgeRepository(IMongoContext context, IMemoryCache memoryCache, ILogger<EntityNodeEdgeRepository> logger, string path) : this(context, memoryCache, logger)
    {
        _path = path;
    }

    public string CollectionName { get; protected set; }

    public async Task<IEnumerable<EntityNode>> QueryIntersectingGeoFences(GeoJsonPointFeature point)
    {
        var models = await LoadJsonAsync();
        
        var coordinate = point.Coordinates.ToCoordinate();
        var geometry = Geometry.DefaultFactory.CreatePoint(coordinate);
        var geoFence = models.Where(m => (m.GeoFence != null)).ToArray();
        return geoFence.Where(m => m.GeoFence != null && m.GeoFence.ToPolygon().Intersects(geometry)).ToArray();
    }
    
    public async Task<IEnumerable<EntityNode>> QueryIntersectingIntersections(GeoJsonLineStringFeature route)
    {
        return await QueryIntersectingByType("Intersection", route);
    }
    
    public async Task<IEnumerable<EntityNode>> QueryIntersectingApproaches(GeoJsonLineStringFeature route)
    {
        return await QueryIntersectingGeometryLineStringByType("Approach", route);
    }
    
    public async Task<IEnumerable<EntityNode>> QueryIntersectingStreetSegment(GeoJsonPointFeature point)
    {
        var models = await LoadJsonAsync();
        var coordinate = point.Coordinates.ToCoordinate();
        var geometry = Geometry.DefaultFactory.CreatePoint(coordinate);
        return models
            .Where(m => m.GeoFence != null  && m.Type.Id == StreetSegmentTypeId.Id)
            .Where(m => m.GeoFence != null && m.GeoFence.ToPolygon().Intersects(geometry));
    }

    private async Task<IEnumerable<EntityNode>> QueryIntersectingByType(string type, GeoJsonLineStringFeature route)
    {
        var models = await LoadJsonAsync();
        var lineString = route.ToLinestring();
        return models
            .Where(m => m.GeoFence != null && m.Type.Name == type)
            .Where(m => m.GeoFence != null && m.GeoFence.ToPolygon().Intersects(lineString));;
    }
    
    private async Task<IEnumerable<EntityNode>> QueryIntersectingGeometryLineStringByType(string type, GeoJsonLineStringFeature route)
    {
        var models = await LoadJsonAsync();
        var lineString = route.ToLinestring();
        return models
            .Where(m => m.Geometry!= null && m.Type.Name == type)
            .Where(m => m.Geometry != null && m.Geometry.LineString.ToLinestring().Intersects(lineString));;
    }

    public async Task SoftDelete(Guid id)
    {
        var model = await GetByIdAsync(id);
        if (model != null)
        {
            //model.IsDeleted = true;
            Update(model);
        }
    }

    public async Task<IEnumerable<EntityNode>> GetAllExceptDeletedAsync()
    {
        var result = await GetAllAsync();
        return result;
    }
    
    public async Task<IEnumerable<EntityNode>> GetByIntersectionIdAsync(Guid id)
    {
        return await GetAllAsync();
    }

    public IDbContext DbContext { get; }
    public void Dispose()
    {
        DbContext.Dispose();
    }

    public void Add(EntityNode document)
    {
        var models = LoadJson().ToList();
        models.Add(document);
        SaveJson(models);
    }

    public async Task<EntityNode?> GetByIdAsync(Guid id)
    {
        var models = await LoadJsonAsync();
        return models.FirstOrDefault(m => m.Id == id) ?? new EntityNode();
    }

    public EntityNode GetById(Guid id)
    {
        return LoadJson().FirstOrDefault(m => m.Id == id) ?? new EntityNode();
    }

    public async Task<IEnumerable<EntityNode>> GetAllAsync()
    {
        return await LoadJsonAsync();
    }

    public IEnumerable<EntityNode> GetAll()
    {
        return LoadJson();
    }

    public void Update(EntityNode document)
    {
        var models = LoadJson();
        var modelsArray = models.ToList();
        var index = modelsArray.FindIndex(m => m.Id == document.Id);
        modelsArray[index] = document;
        SaveJson(modelsArray);
    }

    public void Remove(Guid id)
    {
        var models = LoadJson();
        var result = models.Where(m => m.Id != id);
        SaveJson(result);
    }

    public async Task ReplaceDataAsync(IEnumerable<EntityNode> models)
    {
        await SaveJsonAsync(models);
    }
    
    public async Task<IEnumerable<EntityNode>> LoadJsonAsync()
    {       
        return await _memoryCache.GetOrCreateAsync(
        "nodes",
        cacheEntry =>
        {
            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
            return LoadDataAsync();
        });
    }

    public async Task<IEnumerable<EntityNode>> LoadDataAsync()
    {
        _logger.LogInformation("Loading Data");
        IEnumerable<EntityNode>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = await r.ReadToEndAsync();
        result = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(json, JsonPayloadSerializerOptions.Options);
        return result ?? new List<EntityNode>();
    }

    public IEnumerable<EntityNode> LoadJson()
    {
        IEnumerable<EntityNode>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = r.ReadToEnd();
        result = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(json, JsonPayloadSerializerOptions.Options);
        return result ?? new List<EntityNode>();;
    }
    
    public async Task SaveJsonAsync(IEnumerable<EntityNode> models)
    {
        var json = JsonSerializer.Serialize(models, JsonPayloadSerializerOptions.Options);
        using StreamWriter w = new StreamWriter(_path);
        await w.WriteAsync(json);
    }
    
    private void SaveJson(IEnumerable<EntityNode> models)
    {
        var json = JsonSerializer.Serialize(models, JsonPayloadSerializerOptions.Options);
        using StreamWriter w = new StreamWriter(_path);
        w.Write(json);
    }
}
