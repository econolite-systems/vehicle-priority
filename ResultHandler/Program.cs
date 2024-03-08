// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using ResultHandler;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

await host.RunAsync();
