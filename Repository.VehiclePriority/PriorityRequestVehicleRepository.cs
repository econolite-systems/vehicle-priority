// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Persistence.Common.Repository;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority;

public class PriorityRequestVehicleRepository : StringDocumentRecordRepositoryBase<PriorityRequestVehicleConfiguration>, IPriorityRequestVehicleRepository
{
    public PriorityRequestVehicleRepository(IConfiguration configuration, IMongoContext context, ILogger<PriorityRequestVehicleRepository> logger) : base(context, logger)
    {
    }
}

public interface IPriorityRequestVehicleRepository : IRepository<PriorityRequestVehicleConfiguration, string>
{
}
