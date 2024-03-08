// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Base;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Vehicle.Priority.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient(_ => new ConsumeResultFactory<Guid, Payload>((x) => Guid.Parse(x),
            (string type, string data) => Payload.ToObject<Payload>(data)));
        services.AddTransient<IConsumer<Guid, Payload>, Consumer<Guid, Payload>>(_ =>
            new Consumer<Guid, Payload>(_.GetService<ConsumeResultFactory<Guid, Payload>>(),
                _.GetService<IConfiguration>(), _.GetService<ILogger<Consumer<Guid, Payload>>>()));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
