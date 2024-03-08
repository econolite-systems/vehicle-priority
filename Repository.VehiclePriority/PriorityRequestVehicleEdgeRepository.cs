// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Persistence.Common.Contexts;
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority;

public class PriorityRequestVehicleEdgeRepository : IPriorityRequestVehicleEdgeRepository
{
    private readonly string _path = "./data/vehicle_config.json";
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PriorityRequestVehicleEdgeRepository> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly Guid _intersection;

    public PriorityRequestVehicleEdgeRepository(IConfiguration configuration, IMongoContext context, IMemoryCache memoryCache, ILogger<PriorityRequestVehicleEdgeRepository> logger)
    {
        DbContext = context;
        _memoryCache = memoryCache;
        _logger = logger;
        CollectionName = "PriorityRequestVehicleConfiguration";
        if (!Guid.TryParse(configuration["Intersection"], out _intersection))
        {
            _intersection = Guid.Empty;
        };
    }
    
    public string CollectionName { get; protected set; }
    
    public IDbContext DbContext { get; }
    public void Dispose()
    {
        DbContext.Dispose();
    }

    public void Add(PriorityRequestVehicleConfiguration document)
    {
        throw new NotImplementedException();
    }

    public Task<PriorityRequestVehicleConfiguration?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public PriorityRequestVehicleConfiguration? GetById(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PriorityRequestVehicleConfiguration>> GetAllAsync()
    {
        return await LoadDataAsync();
    }

    public IEnumerable<PriorityRequestVehicleConfiguration> GetAll()
    {
        return LoadJson();
    }

    public void Update(PriorityRequestVehicleConfiguration document)
    {
        throw new NotImplementedException();
    }

    public void Remove(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsVehicleAllowedAsync(string id)
    {
        var configs = await this.GetAllAsync();
        var config = configs.FirstOrDefault();
        var intersection = config?.PriorityRequestVehicleIntersections?.FirstOrDefault((i) => i.Id == _intersection);
        
        if (!config?.Vehicles.Any() ?? true)
        {
            return true;
        }

        var intersectionAllowed = intersection == null ||
               (!intersection.Disable && (intersection.Vehicles.Any((v) => v.Name == id) ||
                                          !intersection.Vehicles.Any()));
        var vehicleAllowed = config?.Vehicles.Any(v => v.Id == id && v.Allowed) ?? true;
        
        return intersectionAllowed && vehicleAllowed;
    }
    
    public async Task<(int level, int priority)> GetLevelPriorityAsync(string id)
    {
        var configs = await this.GetAllAsync();
        var config = configs.FirstOrDefault();
        var priorityRequestVehicle = config?.Vehicles.FirstOrDefault(v => v.Id == id);
        
        if (config == null || priorityRequestVehicle == null)
        {
            return (10,10);
        }

        var level = config.PriorityRequestVehicleClassLevel.FirstOrDefault(l => l.Type == priorityRequestVehicle.Type);
        var priority = config.PriorityRequestVehicleClassType.FirstOrDefault(l => l.Type == priorityRequestVehicle.Type);
        return (level?.Id ?? 10, priority?.Id ?? 10);
    }
    
    public async Task<IEnumerable<PriorityRequestVehicleConfiguration>> LoadDataAsync()
    {
        _logger.LogInformation("Loading Data");
        IEnumerable<PriorityRequestVehicleConfiguration>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = await r.ReadToEndAsync();
        result = JsonSerializer.Deserialize<IEnumerable<PriorityRequestVehicleConfiguration>>(json, _jsonSerializerOptions);
        return result ?? new List<PriorityRequestVehicleConfiguration>();
    }

    public IEnumerable<PriorityRequestVehicleConfiguration> LoadJson()
    {
        IEnumerable<PriorityRequestVehicleConfiguration>? result = null;
        using StreamReader r = new StreamReader(_path);
        var json = r.ReadToEnd();
        result = JsonSerializer.Deserialize<IEnumerable<PriorityRequestVehicleConfiguration>>(json, _jsonSerializerOptions);
        return result ?? new List<PriorityRequestVehicleConfiguration>();;
    }
    
    public async Task SaveJsonAsync(IEnumerable<PriorityRequestVehicleConfiguration> models)
    {
        var json = JsonSerializer.Serialize<IEnumerable<PriorityRequestVehicleConfiguration>>(models, _jsonSerializerOptions);
        using StreamWriter w = new StreamWriter(_path);
        await w.WriteAsync(json);
    }
    
    private void SaveJson(IEnumerable<PriorityRequestVehicleConfiguration> models)
    {
        var json = JsonSerializer.Serialize<IEnumerable<PriorityRequestVehicleConfiguration>>(models, _jsonSerializerOptions);
        using StreamWriter w = new StreamWriter(_path);
        w.Write(json);
    }
}

public interface IPriorityRequestVehicleEdgeRepository : IPriorityRequestVehicleRepository
{
    Task<(int level, int priority)> GetLevelPriorityAsync(string id);
    Task<bool> IsVehicleAllowedAsync(string id);
    Task<IEnumerable<PriorityRequestVehicleConfiguration>> LoadDataAsync();
    Task SaveJsonAsync(IEnumerable<PriorityRequestVehicleConfiguration> models);
}
