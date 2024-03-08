// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Messaging.Extensions;
using Econolite.Ode.Models.VehiclePriority.Algorithm;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics.Extensions;
using Econolite.Ode.Worker.Eta;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMessaging();
        services.AddMetrics(context.Configuration, "Vehicle Priority Eta Worker")
            .AddUserEventSupport(context.Configuration, _ =>
            {
                _.DefaultSource = "Vehicle Priority Eta Worker";
                _.DefaultLogName = Econolite.Ode.Monitoring.Events.LogName.SystemEvent;
                _.DefaultCategory = Econolite.Ode.Monitoring.Events.Category.Server;
                _.DefaultTenantId = Guid.Empty;
            });
        services.AddTransient<IPayloadSpecialist<PriorityRequest>, JsonPayloadSpecialist<PriorityRequest>>();
        services.AddTransient<IConsumeResultFactory<Guid, PriorityRequest>, ConsumeResultFactory<PriorityRequest>>();
        services.AddTransient<IConsumer<Guid, PriorityRequest>, Consumer<Guid, PriorityRequest>>();
        services.AddTransient<IProducer<Guid, PriorityResponse>, Producer<Guid, PriorityResponse>>();
        services.Configure<MessageFactoryOptions<PriorityResponse>>(_ =>
        {
            _.FuncBuildPayloadElement = element => new BaseJsonPayload<PriorityResponse>(element);
        });
        services.AddTransient<IMessageFactory<Guid, PriorityResponse>, MessageFactory<PriorityResponse>>();
        services.AddHostedService<EtaWorker>();
    })
    .Build();

await host.RunAsync();
