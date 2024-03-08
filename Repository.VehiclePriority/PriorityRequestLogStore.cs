// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Econolite.Ode.Repository.VehiclePriority.Records;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority
{
    public class PriorityRequestLogStore : TimestampedIndexedRecordRepositoryBase<PriorityRequestLog>, IPriorityRequestLogStore
    {
        public PriorityRequestLogStore(IMongoContext context, ILogger<PriorityRequestLogStore> logger) : base(context, logger)
        {

        }

        public async Task InsertAsync(Guid deviceId, PriorityRequestMessage priorityRequestMessage)
        {
            await AddAsync(priorityRequestMessage.ToPrioritoryRequestLog(deviceId, DateTime.UtcNow));
        }
    }
}
