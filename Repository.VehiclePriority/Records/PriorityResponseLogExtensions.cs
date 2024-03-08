// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using MongoDB.Bson;

namespace Econolite.Ode.Repository.VehiclePriority.Records
{
    static public class PriorityResponseLogExtensions
    {
        static public PriorityResponseLog ToPriorityResponseLog(this PriorityResponseMessage priorityResponseMessage, Guid deviceId, DateTime timestamp)
        {
            return new PriorityResponseLog(ObjectId.Empty)
            {
                Timestamp = timestamp,
                DeviceId = deviceId,
                PriorityResponseMessage = priorityResponseMessage,
            };
        }
    }
}
