// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Econolite.Ode.Repository.VehiclePriority.Records;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority
{
    public class PriorityResponseLogStore : TimestampedIndexedRecordRepositoryBase<PriorityResponseLog>, IPriorityResponseLogStore
    {
        public PriorityResponseLogStore(IMongoContext context, ILogger<PriorityResponseLogStore> logger) : base(context, logger)
        {

        }

        public async Task InsertAsync(Guid deviceId, PriorityResponseMessage priorityResponseMessage)
        {
            await AddAsync(priorityResponseMessage.ToPriorityResponseLog(deviceId, DateTime.UtcNow));
        }
    }
}
