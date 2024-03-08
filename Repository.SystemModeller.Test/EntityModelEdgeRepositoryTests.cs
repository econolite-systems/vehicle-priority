// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
// ReSharper disable HeapView.BoxingAllocation
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.Entities.Spatial;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Persistence.Mongo;
using Econolite.Ode.Persistence.Mongo.Client;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.OdeRepository.SystemModeller;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;


namespace Econolite.Ode.Repository.SystemModeler.Test;

public class EntityModelEdgeRepositoryTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<EntityNodeEdgeRepository> _logger = Mock.Of<ILogger<EntityNodeEdgeRepository>>();
    private readonly IMongoContext _fixtureContext;
    private readonly EntityNodeEdgeRepository _repo;

    public EntityModelEdgeRepositoryTests()
    {
        _memoryCache = new MemoryCacheMock();
        _fixtureContext = new MongoContext(new ClientProvider(new OptionsWrapper<MongoOptions>(new MongoOptions()), new OptionsWrapper<MongoConnectionStringOptions>(new MongoConnectionStringOptions()), new NullLoggerFactory()), new NullLoggerFactory());
        _repo = CreateRepository();
    }

    [Fact]
    public void LoadEntityNodesTest()
    {
        var result = _repo.GetAll();
    }

    [Fact]
    public async Task Intersection_GeoFence_Test()
    {
        var result = await _repo.QueryIntersectingGeoFences(new GeoJsonPointFeature() { Coordinates = new []{
            -83.0473172763813,
            42.52078881852921
            }});

        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Type.Id == IntersectionTypeId.Id || m.Type.Id == SignalTypeId.Id);
    }
    
    [Fact]
    public async Task IntersectionQuery_GeoFence_Test()
    {
        var result = await _repo.QueryIntersectingIntersections(new GeoJsonLineStringFeature(){
            Coordinates = new[]
            {
                new[] {-83.04714999682335, 42.520481142636385},
                new[] {-83.04718183220778, 42.521187986147794}
            }
        });

        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Type.Id == IntersectionTypeId.Id || m.Type.Id == SignalTypeId.Id);
    }
    
    [Fact]
    public async Task ApproachQuery_GeoFence_Test()
    {
        var result = await _repo.QueryIntersectingApproaches(new GeoJsonLineStringFeature(){
            Coordinates = new[]
            {
                new[] {-83.04714999682335, 42.520481142636385},
                new[] {-83.04718183220778, 42.521187986147794}
            }
        });

        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Type.Id == ApproachTypeId.Id);
    }
    
    [Fact]
    public async Task StreetSegment_GeoFence_Test()
    {
        var result = await _repo.QueryIntersectingGeoFences(new GeoJsonPointFeature() { Coordinates = new []{
            -83.04660683387056,
            42.51484487059864
        }});

        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Type.Id == StreetSegmentTypeId.Id);
    }
    
    [Fact]
    public async Task StreetSegmentQuery_Test()
    {
        var result = await _repo.QueryIntersectingStreetSegment(new GeoJsonPointFeature() { Coordinates = new []{
            -83.04660683387056,
            42.51484487059864
        }});

        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Type.Id == StreetSegmentTypeId.Id);
    }

    protected Guid Id { get; } = Guid.NewGuid();
    
    protected EntityNodeEdgeRepository CreateRepository()
    {
        return new EntityNodeEdgeRepository(_fixtureContext, _memoryCache, _logger, "./data/config.json");
    }

    protected EntityModel CreateDocument()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
            
        var approach = new EntityModel()
        {
            EntityType = "Approach",
            Geometry = new GeoJsonGeometry()
            {
                LineString = new GeoJsonLineStringFeature()
                {
                    Coordinates = new[] {
                        new[] {-111.89123779535294, 40.760442523425205},
                        new[] {-111.89096823334694, 40.76043642853944}
                    },
                    Properties = new GeoJsonProperties()
                    {
                        Intersection = Guid.NewGuid(),
                        Bearing = Bearing.NB,
                        Phases = new []
                        {
                            new PhaseModel()
                            {
                                Number = 2,
                                Movement = Movement.Thru.ToString(),
                                Lanes = 2,
                                Detectors = new []
                                {
                                    new DetectorModel()
                                    {
                                        Advanced = true
                                    }
                                }
                            },
                            new PhaseModel()
                            {
                                Number = 1,
                                Movement = Movement.Left.ToString(),
                                Lanes = 1
                            }
                        }
                    }
                }
            },
            // Geometry = JsonSerializer.SerializeToDocument(new GeoJsonLineString(){
            //     Coordinates = 
            //     Type = "LineString"
            // }, jsonOptions),
            // Properties = JsonSerializer.SerializeToDocument(new ApproachPropertiesModel{Intersection = Guid.NewGuid().ToString(), Bearing = Bearing.NB.ToString(), Phases = new []{
            //     new PhaseModel(){ Number = 2, Movement = Movement.Thru.ToString(), Lanes = 2, Detectors = new []{new DetectorModel(){Advanced = true}}},
            //     new PhaseModel(){ Number = 1, Movement = Movement.Left.ToString(), Lanes = 1}}},jsonOptions)
        };

        return approach;
    }
}

public class MemoryCacheMock : IMemoryCache
{
    public void Dispose()
    {
    }

    public bool TryGetValue(object key, out object value)
    {
        value = LoadData();
        return true;
    }

    public ICacheEntry CreateEntry(object key)
    {
        throw new NotImplementedException();
    }

    public void Remove(object key)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EntityNode>> GetOrCreateAsync(object key, Func<ICacheEntry, Task<IEnumerable<EntityNode>>> factory)
    {
        return await LoadDataAsync();
    }
    
    private async Task<IEnumerable<EntityNode>> LoadDataAsync()
    {
        IEnumerable<EntityNode>? result = null;
        using StreamReader r = new StreamReader("./data/config.json");
        var json = await r.ReadToEndAsync();
        result = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(json, JsonPayloadSerializerOptions.Options);
        return result ?? new List<EntityNode>();
    }
    
    private IEnumerable<EntityNode> LoadData()
    {
        IEnumerable<EntityNode>? result = null;
        using StreamReader r = new StreamReader("./data/config.json");
        var json = r.ReadToEnd();
        result = JsonSerializer.Deserialize<IEnumerable<EntityNode>>(json, JsonPayloadSerializerOptions.Options);
        return result ?? new List<EntityNode>();
    }
}
