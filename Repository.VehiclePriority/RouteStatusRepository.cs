// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.Entities;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Econolite.Ode.Repository.VehiclePriority;

public class RouteStatusRepository : GuidDocumentRepositoryBase<RouteStatus>, IRouteStatusRepository
{
    public RouteStatusRepository(IMongoContext context, ILogger<RouteStatusRepository> logger) : base(context, logger)
    {
    }
    
    public async Task<RouteStatus?> GetByRouteIdAsync(Guid id)
    {
        var filter = MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => r.RouteId == id);
        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync<RouteStatus>(filter));
        return results.FirstOrDefault();
    }
    
    public async Task<RouteStatus?> GetByVehicleIdAsync(string id)
    {
        var filter = MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => r.VehicleId == id);
        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync<RouteStatus>(filter));
        return results.FirstOrDefault();
    }
    
    public async Task<IEnumerable<RouteStatus>> GetByVehicleAndTripPointAsync(string id, TripPointLocation location)
    {
        var filter = MongoDB.Driver.Builders<RouteStatus>.Filter.And(
            MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => r.VehicleId == id),
            MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => r.Location == location));
        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync<RouteStatus>(filter));
        return await results.ToListAsync();
    }

    public async Task<int> GetNewTaskId(Guid intersectionId)
    {
        var filter = MongoDB.Driver.Builders<RouteStatus>.Filter.And(
            MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => r.NextIntersection != null && r.NextIntersection.IntersectionId == intersectionId),
            MongoDB.Driver.Builders<RouteStatus>.Filter.Where(r => !r.Completed));
        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync<RouteStatus>(filter));
        var running = await results.ToListAsync();
        var result = 1;
        if (running.Any())
        {
            result = Enumerable.Range(1, 255).First(r => running.TrueForAll(s => s.RequestId != r));
        }
        return result;
    }
}
