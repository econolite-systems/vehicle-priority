// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Diagnostics;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority.Algorithm;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using ETA_Predictor;

namespace Econolite.Ode.Worker.Eta;

public class EtaWorker : BackgroundService
{
    private const string PriorityRequestType = nameof(PriorityRequest);
    private readonly IConsumer<Guid, PriorityRequest> _consumer;
    private readonly IProducer<Guid, PriorityResponse> _producer;
    private readonly ILogger<EtaWorker> _logger;
    private readonly IMessageFactory<Guid, PriorityResponse> _messageFactory;
    private readonly IMetricsCounter _loopCounter;

    private readonly string _producerTopic;
    private readonly UserEventFactory _userEventFactory;

    public EtaWorker(IConfiguration configuration, IConsumer<Guid, PriorityRequest> consumer, IProducer<Guid, PriorityResponse> producer, ILogger<EtaWorker> logger, IMessageFactory<Guid, PriorityResponse> messageFactory, IMetricsFactory metricsFactory, UserEventFactory userEventFactory)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;
        var topic = configuration["Topics:Priority.Request"] ?? throw new NullReferenceException("Topics:Priority.Request missing in config.");
        _producerTopic = configuration["Topics:Priority.Response"] ?? throw new NullReferenceException("Topics:Priority.Response missing in config.");
        _consumer.Subscribe(topic);
        _logger.LogInformation("Subscribed topic {@}", topic);
        _messageFactory = messageFactory;
        _userEventFactory = userEventFactory;

        _loopCounter = metricsFactory.GetMetricsCounter("Eta");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                Debug.Assert(result is not null);

                try
                {
                    var task = result.Type switch
                    {
                        PriorityRequestType => ProcessPriorityRequestAsync(result),
                        _ => Task.CompletedTask
                    };

                    await task;
                    _consumer.Complete(result);

                    _loopCounter.Increment();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Unhandled exception while processing: {@MessageType}", result.Type);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker.Eta stopping");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Exception thrown while processing priority request messages");
        }
    }

    private async Task ProcessPriorityRequestAsync(ConsumeResult<Guid, PriorityRequest> result)
    {
        var request = result.ToObject<PriorityRequest>();
        var results = EtaCalculator.GetEta(request.VehicleData, request.RealTimeData, request.HistoricalData, request.RoadwayData);
        await _producer.ProduceAsync(_producerTopic, _messageFactory.Build(
            result.TenantId,
            result.DeviceId!.Value,
            new PriorityResponse { Result = results }));

        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Priority ETA response sent for request for device: {0}", result.DeviceId)));
    }
}
