// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Persistence.Mongo.Context;
using Econolite.Ode.Persistence.Mongo.Repository;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Repository.VehiclePriority;

public class RegisteredRouteRepository : GuidDocumentRepositoryBase<RegisteredRoute>, IRegisteredRouteRepository
{
    public RegisteredRouteRepository(IMongoContext context, ILogger<RegisteredRouteRepository> logger) : base(context, logger)
    {
    }
}
