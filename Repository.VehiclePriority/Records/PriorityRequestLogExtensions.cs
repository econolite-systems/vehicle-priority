// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Records;
using MongoDB.Bson;

namespace Econolite.Ode.Repository.VehiclePriority.Records
{
    public static class PriorityRequestLogExtensions
    {
        static public PriorityRequestLog ToPrioritoryRequestLog(this PriorityRequestMessage priorityResquestMessage, Guid deviceId, DateTime timestamp)
        {
            var result = new PriorityRequestLog(ObjectId.Empty)
            {
                Timestamp = timestamp,
                DeviceId = deviceId,
                PriorityRequestMessage = priorityResquestMessage,
            };
            return result;
        }
    }
}
