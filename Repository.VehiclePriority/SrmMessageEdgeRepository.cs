// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Client.VehiclePriority.Model.J2735;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority;

public class SrmMessageEdgeRepository : ISrmMessageRepository
{
    private readonly string _path = "./data/srm_status.json";
    private readonly ILogger<SrmMessageEdgeRepository> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    public SrmMessageEdgeRepository(ILogger<SrmMessageEdgeRepository> logger)
    {
        _logger = logger;
    }

    public async Task<SrmMessage?> GetByIdAsync(string id)
    {
        var models = await LoadJsonAsync();
        return models.FirstOrDefault(m => m.SrmMessageContent.FirstOrDefault()?.Srm.Requestor.Id.VehicleId == id) ?? new SrmMessage();
    }

    public SrmMessage GetById(string id)
    {
        return LoadJson().FirstOrDefault(m => m.SrmMessageContent.FirstOrDefault()?.Srm.Requestor.Id.VehicleId == id) ?? new SrmMessage();
    }

    public async Task<IEnumerable<SrmMessage>> GetAllAsync()
    {
        return await LoadJsonAsync();
    }

    public IEnumerable<SrmMessage> GetAll()
    {
        return LoadJson();
    }

    public void Update(SrmMessage document)
    {
        var models = LoadJson();
        var modelsArray = models.ToList();
        var index = modelsArray.FindIndex(m => m.SrmMessageContent.FirstOrDefault()?.Srm.Requestor.Id.VehicleId == document.SrmMessageContent.FirstOrDefault()?.Srm.Requestor.Id.VehicleId);
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

    public void Remove(string id)
    {
        var models = LoadJson();
        var result = models.Where(m => m.SrmMessageContent.FirstOrDefault()?.Srm.Requestor.Id.VehicleId != id);
        SaveJson(result);
    }
    
    public async Task ReplaceDataAsync(IEnumerable<SrmMessage> models)
    {
        await SaveJsonAsync(models);
    }

    private async Task<IEnumerable<SrmMessage>> LoadJsonAsync()
    {
        FileExists();
        IEnumerable<SrmMessage>? result = null;
        using var r = new StreamReader(_path);
        var json = await r.ReadToEndAsync();
        result = JsonSerializer.Deserialize<IEnumerable<SrmMessage>>(json, _jsonSerializerOptions);
        return result ?? new List<SrmMessage>();
    }

    private IEnumerable<SrmMessage> LoadJson()
    {
        FileExists();
        IEnumerable<SrmMessage>? result = null;
        using var r = new StreamReader(_path);
        var json = r.ReadToEnd();
        result = JsonSerializer.Deserialize<IEnumerable<SrmMessage>>(json, _jsonSerializerOptions);
        return result ?? new List<SrmMessage>();;
    }
    
    private async Task SaveJsonAsync(IEnumerable<SrmMessage> models)
    {
        FileExists();
        var json = JsonSerializer.Serialize<IEnumerable<SrmMessage>>(models, _jsonSerializerOptions);
        await using var w = new StreamWriter(_path);
        await w.WriteAsync(json);
    }
    
    private void SaveJson(IEnumerable<SrmMessage> models)
    {
        FileExists();
        var json = JsonSerializer.Serialize<IEnumerable<SrmMessage>>(models, _jsonSerializerOptions);
        using var w = new StreamWriter(_path);
        w.Write(json);
    }
    
    private void FileExists()
    {
        if (!File.Exists(_path))
        {
            using var w = new StreamWriter(_path);
            w.Write("[]");
        }
    }
}
