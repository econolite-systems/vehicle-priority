// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Records;
using MongoDB.Bson;

namespace Econolite.Ode.Repository.VehiclePriority.Records
{
    public record PriorityResponseLog(ObjectId Id) : TimestampedIndexedRecordBase(Id)
    {
        public Guid DeviceId { get; set; }
        public PriorityResponseMessage PriorityResponseMessage { get; set; } = new PriorityResponseMessage(new PriorityResponse(0, 0, 0, 0, 0, ""));
    }
}
