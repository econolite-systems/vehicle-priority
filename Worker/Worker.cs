// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Base;

namespace Econolite.Ode.Vehicle.Priority.Worker;

public class Worker : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IConsumer<Guid, Payload> _consumer;
    private readonly ILogger<Worker> _logger;

    public Worker(IConsumer<Guid, Payload> consumer, IConfiguration configuration, ILogger<Worker> logger)
    {
        _consumer = consumer;
        _configuration = configuration;
        _logger = logger;

        var topic = _configuration["Topics:Priority"];

        _consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed topic {@}", topic);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Process(cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task Process(CancellationToken cancellationToken)
    {
        try
        {
            var numberprocessed = 0;
            while (true)
            {
                var consumeresult = _consumer.Consume(cancellationToken);
                _logger.LogTrace("{@}",
                    new {Processed = ++numberprocessed, consumeresult.TenantId, consumeresult.Type});
                _consumer.Complete(consumeresult);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Bad things");
        }

        return Task.CompletedTask;
    }
}
