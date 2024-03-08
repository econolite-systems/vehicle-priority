// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority;

public class RouteStatusEdgeRepository : IRouteStatusRepository
{
    private readonly string _path = "./data/vehicle_status.json";
    private readonly ILogger<RouteStatusEdgeRepository> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    public RouteStatusEdgeRepository(IMongoContext context, ILogger<RouteStatusEdgeRepository> logger)
    {
        DbContext = context;
        _logger = logger;
    }
    
    public RouteStatusEdgeRepository(IMongoContext context, ILogger<RouteStatusEdgeRepository> logger, string path) : this(context, logger)
    {
        _path = path;
    }
    
    public IDbContext DbContext { get; }
    public void Dispose()
    {
        DbContext.Dispose();
    }

    public void Add(RouteStatus document)
    {
        var models = LoadJson().ToList();
        models.Add(document);
        SaveJson(models);
    }

    public async Task<RouteStatus?> GetByIdAsync(Guid id)
    {
        var models = await LoadJsonAsync();
        return models.FirstOrDefault(m => m.Id == id) ?? new RouteStatus();
    }

    public RouteStatus GetById(Guid id)
    {
        return LoadJson().FirstOrDefault(m => m.Id == id) ?? new RouteStatus();
    }

    public async Task<IEnumerable<RouteStatus>> GetAllAsync()
    {
        return await LoadJsonAsync();
    }

    public IEnumerable<RouteStatus> GetAll()
    {
        return LoadJson();
    }

    public void Update(RouteStatus document)
    {
        var models = LoadJson();
        var modelsArray = models.ToList();
        var index = modelsArray.FindIndex(m => m.Id == document.Id);
        if (index >= 0)
        {
            modelsArray[index] = document;
        }
        else
        {
            modelsArray.Add(document);
        }
        
        SaveJson(modelsArray);
    }

    public void Remove(Guid id)
    {
        var models = LoadJson();
        var result = models.Where(m => m.Id != id);
        SaveJson(result);
    }
    
    public async Task<RouteStatus?> GetByRouteIdAsync(Guid id)
    {
        var models = await LoadJsonAsync();
        return models.FirstOrDefault(m => m.RouteId == id);
    }
    
    public async Task<RouteStatus?> GetByVehicleIdAsync(string id)
    {
        var models = await LoadJsonAsync();
        return models.FirstOrDefault(m => m.VehicleId == id && !m.Completed);
    }
    
    public async Task<IEnumerable<RouteStatus>> GetByVehicleAndTripPointAsync(string id, TripPointLocation location)
    {
        var models = await LoadJsonAsync();
        return models.Where(m => m.VehicleId == id && m.Location == location);
    }
    
    public async Task<int> GetNewTaskId(Guid intersectionId)
    {
        var filter = MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => !r.Completed);
        var results = await LoadJsonAsync();
        var running = results.ToList();
        var result = 1;
        if (running.Any())
        {
            result = Enumerable.Range(1, 255).First(r => running.TrueForAll(s => s.RequestId != r));
        }
        return result;
    }
    
    public async Task ReplaceDataAsync(IEnumerable<RouteStatus> models)
    {
        await SaveJsonAsync(models);
    }

    private async Task<IEnumerable<RouteStatus>> LoadJsonAsync()
    {
        IEnumerable<RouteStatus>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = await r.ReadToEndAsync();
        result = JsonSerializer.Deserialize<IEnumerable<RouteStatus>>(json, _jsonSerializerOptions);
        return result ?? new List<RouteStatus>();
    }

    private IEnumerable<RouteStatus> LoadJson()
    {
        IEnumerable<RouteStatus>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = r.ReadToEnd();
        result = JsonSerializer.Deserialize<IEnumerable<RouteStatus>>(json, _jsonSerializerOptions);
        return result ?? new List<RouteStatus>();;
    }
    
    private async Task SaveJsonAsync(IEnumerable<RouteStatus> models)
    {
        var json = JsonSerializer.Serialize<IEnumerable<RouteStatus>>(models, _jsonSerializerOptions);
        await using StreamWriter w = new StreamWriter(_path);
        await w.WriteAsync(json);
    }
    
    private void SaveJson(IEnumerable<RouteStatus> models)
    {
        var json = JsonSerializer.Serialize<IEnumerable<RouteStatus>>(models, _jsonSerializerOptions);
        using StreamWriter w = new StreamWriter(_path);
        w.Write(json);
    }


}
