// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.OdeRepository.SystemModeller;

public static class EntityModelRepositoryExtensions
{
    public static IServiceCollection AddSystemModellerEdgeRepo(this IServiceCollection services)
    {
        services.AddTransient<IMongoContext, StandInMongoContext>();
        services.AddSingleton<IEntityNodeJsonFileRepository, EntityNodeEdgeRepository>();

        return services;
    }
}
