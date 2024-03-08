// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Econolite.Ode.Repository.VehiclePriority
{
    public interface IPriorityResponseLogStore
    {
        Task InsertAsync(Guid deviceId,  PriorityResponseMessage priorityResponseMessage);
    }
}
