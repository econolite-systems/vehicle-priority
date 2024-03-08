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

public class VehicleLocationStatusRepository : StringDocumentRecordRepositoryBase<VehicleLocationStatus>, IVehicleLocationStatusRepository
{
    private readonly int _minutesToExpireStatus;

    public VehicleLocationStatusRepository(IConfiguration configuration, IMongoContext context, ILogger<VehicleLocationStatusRepository> logger) : base(context, logger)
    {
        var config = configuration["MinutesToExpireStatus"];
        _minutesToExpireStatus = config != null ? int.Parse(config) : 1;
    }

    public async Task UpdateStatus(VehicleLocationStatus status)
    {
        var filter = MongoDB.Driver.Builders<VehicleLocationStatus>.Filter.Eq(v => v.Id, status.Id );
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

    public async Task<IEnumerable<VehicleLocationStatus>> RemoveOldStatusAsync()
    {
        var filter = MongoDB.Driver.Builders<VehicleLocationStatus>.Filter.Lt(v => v.Timestamp, DateTime.UtcNow.AddMinutes(-_minutesToExpireStatus).ToFileTimeUtc());

        var results = await ExecuteDbSetFuncAsync(collection => collection.FindAsync(filter));
        var toRemove = results.ToList();
        foreach (var status in toRemove)
        {
            this.Remove(status.Id);
        }

        if (toRemove.Any())
        {
            await this.DbContext.SaveChangesAsync();
        }
        
        return toRemove;
    }
}
