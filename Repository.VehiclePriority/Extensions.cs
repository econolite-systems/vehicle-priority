// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Repository.VehiclePriority;

public static class Extensions
{
    public static IServiceCollection AddVehiclePriorityRepos(this IServiceCollection services)
    {
        services.AddScoped<IRoutePriorityStatusRepository, RoutePriorityStatusRepository>();
        services.AddScoped<IVehicleLocationStatusRepository, VehicleLocationStatusRepository>();
        services.AddScoped<IRegisteredRouteRepository, RegisteredRouteRepository>();
        services.AddScoped<IRouteStatusRepository, RouteStatusRepository>();
        services.AddScoped<IPriorityRequestVehicleRepository, PriorityRequestVehicleRepository>();

        return services;
    }
    
    public static IServiceCollection AddVehiclePriorityEdgeRepos(this IServiceCollection services)
    {
        services.AddSingleton<IRouteStatusRepository, RouteStatusEdgeRepository>();
        services.AddSingleton<IPriorityRequestVehicleEdgeRepository, PriorityRequestVehicleEdgeRepository>();
        services.AddSingleton<ISrmMessageRepository, SrmMessageEdgeRepository>();
        return services;
    }

    public static IServiceCollection AddVehicleRequestLogRespository(this IServiceCollection services) =>
        services.AddSingleton<IPriorityRequestLogStore, PriorityRequestLogStore>()
        .AddSingleton<IPriorityResponseLogStore, PriorityResponseLogStore>();
}
