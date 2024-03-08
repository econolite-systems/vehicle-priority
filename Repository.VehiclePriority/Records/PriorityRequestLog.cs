// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Records;
using MongoDB.Bson;

namespace Econolite.Ode.Repository.VehiclePriority.Records
{
    public record PriorityRequestLog(ObjectId Id) : TimestampedIndexedRecordBase(Id)
    {
        public Guid DeviceId { get; set; }
        public PriorityRequestMessage PriorityRequestMessage { get; set; } = new PriorityRequestMessage(PriorityRequestType.Initial, new PRequest(0, "", 0, 0, 0, 0, 0));
    }
}
