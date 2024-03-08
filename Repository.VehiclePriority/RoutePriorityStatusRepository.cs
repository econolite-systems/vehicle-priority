// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Status;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NetTopologySuite.Index.HPRtree;

namespace Econolite.Ode.Repository.VehiclePriority;

public class RoutePriorityStatusRepository : GuidDocumentRecordRepositoryBase<RoutePriorityStatus>, IRoutePriorityStatusRepository
{
    private readonly int _minutesToExpireStatus;

    public RoutePriorityStatusRepository(IConfiguration configuration, IMongoContext context, ILogger<RoutePriorityStatusRepository> logger) : base(context, logger)
    {
        var config = configuration["MinutesToExpireStatus"];
        _minutesToExpireStatus = config != null ? int.Parse(config) : 1;
    }

    public async Task UpdateStatus(RoutePriorityStatus status)
    {
        var filter = MongoDB.Driver.Builders<RoutePriorityStatus>.Filter.Eq(v => v.Id, status.Id );
        var count = await ExecuteDbSetFuncAsync(collection => collection.CountDocumentsAsync(filter));
        if (count > 0)
        {
             this.Update(status);
        }
        else
        {
            this.Add(status);    
        }

        var (success, errors) = await this.DbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoutePriorityStatus>> RemoveOldStatusAsync()
    {
        var filter = MongoDB.Driver.Builders<RoutePriorityStatus>.Filter.Lt(v => v.Timestamp, DateTime.UtcNow.AddMinutes(-_minutesToExpireStatus).ToFileTimeUtc() );

        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(filter));
        var toUpdate = results.ToList();
        foreach (var status in toUpdate)
        {
            
            this.Update(status);
        }

        if (toUpdate.Any())
        {
            await this.DbContext.SaveChangesAsync();
        }
        
        return toUpdate;
    }
}

public interface IRoutePriorityStatusRepository
{
    Task UpdateStatus(RoutePriorityStatus status);
    Task<IEnumerable<RoutePriorityStatus>> RemoveOldStatusAsync();
}
