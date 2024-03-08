// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;

namespace Econolite.Ode.Repository.VehiclePriority
{
    public interface IPriorityRequestLogStore
    {
        Task InsertAsync(Guid deviceId, PriorityRequestMessage priorityRequestMessage);
    }
}
