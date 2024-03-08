// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Domain.SystemModeller.Cloud;
using Econolite.Ode.Domain.SystemModeller.Edge;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Models.Entities.Interfaces;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.OdeRepository.SystemModeller;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Domain.SystemModeller.Extensions;

public static class Defined
{
    public static IServiceCollection AddIntersectionConfigUpdate(this IServiceCollection services, Action<SinkOptions<int, GenericJsonResponse>> action) => services
        .AddMessaging()
        .AddMessagingJsonGenericSink(action)
        .AddSingleton<IEntityConfigUpdate, EntityConfigUpdate>();
    
    public static IServiceCollection AddIntersectionConfigResponseWorker(this IServiceCollection services, Action<SinkOptions<int, GenericJsonResponse>> actionSink, Action<SourceOptions<int, EntityNodeConfigRequest>> actionSource) => services
        .AddMessaging()
        .AddMessagingJsonGenericSink(actionSink)
        .AddMessagingJsonGenericSource(_ => int.Parse(Encoding.UTF8.GetString(_)), actionSource)
        .AddHostedService<IntersectionConfigResponseWorker>();
    
    public static IServiceCollection AddIntersectionConfigEdgeWorker(this IServiceCollection services, Action<SinkOptions<int, EntityNodeConfigRequest>> actionSink, Action<SourceOptions<int, GenericJsonResponse>> actionSource) => services
        .AddMessaging()
        .AddMessagingJsonGenericSink(actionSink)
        .AddMessagingJsonGenericSource(_ => int.Parse(Encoding.UTF8.GetString(_)), actionSource)
        .AddHostedService<IntersectionConfigWorker>();

    public static IServiceCollection AddSystemModellerService(this IServiceCollection services) => services
        .AddMemoryCache()
        .AddSingleton<IEntityConfigurationService, EntityConfigurationService>();
    
    public static IServiceCollection AddSystemModellerEdgeService(this IServiceCollection services) => services
        .AddScoped<ISystemModellerService, SystemModellerEdgeService>()
        .AddSingleton<IEntityNodeJsonFileRepository, EntityNodeEdgeRepository>();
}