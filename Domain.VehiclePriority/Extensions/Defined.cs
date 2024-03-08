// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Domain.Asn1.J2735.Extensions;
using System.Text;
using Econolite.Ode.Domain.VehiclePriority.Cloud;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Domain.VehiclePriority.Extensions;

public static class Defined
{
    public static IServiceCollection AddVehiclePriority(this IServiceCollection services)
    {
        services.AddSignalR();
        services.Configure<MessageFactoryOptions<GenericJsonResponse>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<GenericJsonResponse>(_);
        });
        services.AddTransient<IMessageFactory<Guid, GenericJsonResponse>, MessageFactory<GenericJsonResponse>>();
        services.AddTransient<IProducer<Guid, GenericJsonResponse>, Producer<Guid, GenericJsonResponse>>();
        services.AddTransient<IPayloadSpecialist<PriorityRequestMessage>, JsonPayloadSpecialist<PriorityRequestMessage>>();
        services.AddTransient<IConsumeResultFactory<Guid, PriorityRequestMessage>, ConsumeResultFactory<PriorityRequestMessage>>();
        services.AddTransient<IPayloadSpecialist<VehicleUpdate>, JsonPayloadSpecialist<VehicleUpdate>>();
        services.AddTransient<IConsumeResultFactory<Guid, VehicleUpdate>, ConsumeResultFactory<VehicleUpdate>>();
        services.AddTransient<IPayloadSpecialist<PriorityStatusMessage>, JsonPayloadSpecialist<PriorityStatusMessage>>();
        services.AddTransient<IConsumeResultFactory<Guid, PriorityStatusMessage>, ConsumeResultFactory<PriorityStatusMessage>>();
        services.Configure<MessageFactoryOptions<PriorityRequestMessage>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<PriorityRequestMessage>(_);
        });
        services.AddTransient<IMessageFactory<Guid, PriorityRequestMessage>, MessageFactory<PriorityRequestMessage>>();
        services.AddTransient<Messaging.IConsumer<Guid, VehicleUpdate>, Consumer<Guid, VehicleUpdate>>();
        services.AddTransient<Messaging.IConsumer<Guid, PriorityStatusMessage>, Consumer<Guid, PriorityStatusMessage>>();
        services.AddTransient<Messaging.IProducer<Guid, PriorityRequestMessage>, Producer<Guid, PriorityRequestMessage>>();
        services.AddSingleton<IVehiclePriorityPublisher, VehiclePriorityPublisher>();
        services.AddScoped<IVehiclePriorityService, VehiclePriorityService>();
        services.AddSingleton<VehiclePriorityVehicleStatusHub>();
        services.AddHostedService<VehiclePriorityStatusIngesterWorker>();
        services.AddHostedService<PrioritySignalStatusIngesterWorker>();
        return services;
    }
    
    public static IServiceCollection AddVehiclePriorityEdgeService(this IServiceCollection services, Action<SourceOptions<PriorityResponseConfigurationMessage>> configPriorityResponseOptions)
    {
        
        services.AddMessaging();
        services.AddMessagingJsonSource(configPriorityResponseOptions);
        services.Configure<MessageFactoryOptions<RawSsmMessageJsonResponse>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<RawSsmMessageJsonResponse>(_);
        });
        services.AddTransient<IMessageFactory<Guid, RawSsmMessageJsonResponse>, MessageFactory<RawSsmMessageJsonResponse>>();
        services.AddTransient<Messaging.IProducer<Guid, RawSsmMessageJsonResponse>, Producer<Guid, RawSsmMessageJsonResponse>>();
        services.Configure<MessageFactoryOptions<VehicleUpdate>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<VehicleUpdate>(_);
        });
        services.AddTransient<IMessageFactory<Guid, VehicleUpdate>, MessageFactory<VehicleUpdate>>();
        services.AddTransient<Messaging.IProducer<Guid, VehicleUpdate>, Producer<Guid, VehicleUpdate>>();
        services.Configure<MessageFactoryOptions<PriorityRequestMessage>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<PriorityRequestMessage>(_);
        });
        services.AddTransient<IMessageFactory<Guid, PriorityRequestMessage>, MessageFactory<PriorityRequestMessage>>();
        services.AddTransient<Messaging.IProducer<Guid, PriorityRequestMessage>, Producer<Guid, PriorityRequestMessage>>();
        services.Configure<MessageFactoryOptions<PriorityStatusMessage>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<PriorityStatusMessage>(_);
        });
        services.AddTransient<IMessageFactory<Guid, PriorityStatusMessage>, MessageFactory<PriorityStatusMessage>>();
        services.AddTransient<Messaging.IProducer<Guid, PriorityStatusMessage>, Producer<Guid, PriorityStatusMessage>>();
        services.Configure<MessageFactoryOptions<PriorityResponseMessage>>(_ =>
        {
            _.FuncBuildPayloadElement = _ => new BaseJsonPayload<PriorityResponseMessage>(_);
        });
        services.AddTransient<IMessageFactory<Guid, PriorityResponseMessage>, MessageFactory<PriorityResponseMessage>>();
        services.AddTransient<Messaging.IProducer<Guid, PriorityResponseMessage>, Producer<Guid, PriorityResponseMessage>>();
        services.AddSingleton<ICvFiltering, CvFiltering>();
        services.AddSingleton<IVehiclePriorityService, VehiclePriorityEdgeService>();
        services.AddSingleton<IVehiclePriorityEdgePublisher, VehiclePriorityEdgePublisher>();
        //services.AddHostedService<VehiclePriorityEdgeIngesterWorker>();
        services.AddAsn1J2735Service();
        services.AddTransient<VehiclePriorityIngesterHandler>();
        services.AddHostedService<VehiclePriorityEdgeConfigWorker>();
        return services;
    }

    public static IServiceCollection AddPriorityConfigResponseWorker(this IServiceCollection services, Action<SinkOptions<PriorityResponseConfigurationMessage>> actionSink, Action<SourceOptions<int, EntityNodeConfigRequest>> actionSource) => services
        .AddMessaging()
        .AddMessagingJsonSink(actionSink)
        .AddMessagingJsonGenericSource(_ => int.Parse(Encoding.UTF8.GetString(_)), actionSource)
        .AddHostedService<IntersectionConfigResponseWorker>();
    
    public static IServiceCollection AddVehiclePriorityConsumers(this IServiceCollection services, Action<ScalingConsumerOptions>? scalingConsumerOptionsDelegate = null)
    {
        services.AddTransient<IPayloadSpecialist<PriorityRequestMessage>, JsonPayloadSpecialist<PriorityRequestMessage>>();
        services.AddTransient<IConsumeResultFactory<Guid, PriorityRequestMessage>, ConsumeResultFactory<PriorityRequestMessage>>();
        services.AddTransient<Messaging.IConsumer<Guid, PriorityRequestMessage>, Consumer<Guid, PriorityRequestMessage>>();
        services.Configure<ScalingConsumerOptions<Guid, PriorityRequestMessage>>(scalingConsumerOptionsDelegate ?? (_ => { }));
        services.AddTransient<IScalingConsumer<Guid, PriorityRequestMessage>, ScalingConsumer<Guid, PriorityRequestMessage>>();

        services.AddTransient<IPayloadSpecialist<PriorityResponseMessage>, JsonPayloadSpecialist<PriorityResponseMessage>>();
        services.AddTransient<IConsumeResultFactory<Guid, PriorityResponseMessage>, ConsumeResultFactory<PriorityResponseMessage>>();
        services.AddTransient<Messaging.IConsumer<Guid, PriorityResponseMessage>, Consumer<Guid, PriorityResponseMessage>>();
        services.Configure<ScalingConsumerOptions<Guid, PriorityResponseMessage>>(scalingConsumerOptionsDelegate ?? (_ => { }));
        services.AddTransient<IScalingConsumer<Guid, PriorityResponseMessage>, ScalingConsumer<Guid, PriorityResponseMessage>>();

        return services;
    }
}