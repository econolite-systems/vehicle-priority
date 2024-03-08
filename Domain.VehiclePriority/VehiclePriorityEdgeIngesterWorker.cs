// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Client.VehiclePriority.Model.J2735;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Econolite.Ode.Repository.VehiclePriority;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityEdgeIngesterWorker: BackgroundService
{
    private readonly IOptionsMonitor<BsmUdpOptions> _options;
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly IVehiclePriorityEdgePublisher _vehiclePriorityEdgePublisher;
    private readonly ILogger<VehiclePriorityEdgeIngesterWorker> _logger;
    private readonly UserEventFactory _userEventFactory;
    private readonly ISrmMessageRepository _srmMessageRepository;
    private readonly IMetricsCounter _loopCounter;
    private readonly IMetricsCounter _bsmCounter;
    private readonly IMetricsCounter _priorityStatusCounter;
    private readonly IMetricsCounter _priorityResponseCounter;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters ={
            new JsonStringEnumConverter()
        },
    };

    public VehiclePriorityEdgeIngesterWorker(
        IOptionsMonitor<BsmUdpOptions> options,
        IVehiclePriorityService vehiclePriorityService,
        IVehiclePriorityEdgePublisher vehiclePriorityEdgePublisher,
        ILogger<VehiclePriorityEdgeIngesterWorker> logger,
        IMetricsFactory metricsFactory,
        UserEventFactory userEventFactory,
        ISrmMessageRepository srmMessageRepository
        )
    {
        _options = options;
        _vehiclePriorityService = vehiclePriorityService;
        _vehiclePriorityEdgePublisher = vehiclePriorityEdgePublisher;
        _logger = logger;
        _userEventFactory = userEventFactory;
        _srmMessageRepository = srmMessageRepository;

        _loopCounter = metricsFactory.GetMetricsCounter("Edge Bsm Worker");
        _bsmCounter = metricsFactory.GetMetricsCounter("Edge Bsm Count");
        _priorityStatusCounter = metricsFactory.GetMetricsCounter("Edge Priority Status");
        _priorityResponseCounter = metricsFactory.GetMetricsCounter("Edge Priority Response");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            try
            {
                using var udpClient = new UdpClient(_options.CurrentValue.Port);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await ProcessAsync(await udpClient.ReceiveAsync(stoppingToken));

                        _loopCounter.Increment();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception while processing message");
                        _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Error, string.Format("Unhandled BSM data received.")));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Service stopping");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Main processing loop terminated!");
            }
        });
    }

    private async Task ProcessAsync(UdpReceiveResult result)
    {
        var json = Encoding.ASCII.GetString(result.Buffer);
        if (json.Contains("bsm"))
        {
            var bsmMessage = JsonSerializer.Deserialize<BsmMessage>(json, _jsonOptions);
            await ProcessBsmAsync(bsmMessage);
            
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received BSM messages: {0}", bsmMessage?.BsmMessageContent?.Length)));
        }
        else if (json.Contains("srm"))
        {
            var srmMessage = JsonSerializer.Deserialize<SrmMessage>(json, _jsonOptions);
            await ProcessSrmAsync(srmMessage);
            
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received SRM messages: {0}", srmMessage?.SrmMessageContent?.Length)));
        }
        else if (json.Contains("priorityStatus"))
        {
            var priorityStatusMessage = JsonSerializer.Deserialize<PriorityStatusMessage>(json, _jsonOptions);
            await ProcessPriorityStatusAsync(priorityStatusMessage);

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received BSM priority status: {0}", priorityStatusMessage?.PriorityStatus?.Count())));
        }
        else if (json.Contains("priorityResponse"))
        {
            var priorityResponseMessage = JsonSerializer.Deserialize<PriorityResponseMessage>(json, _jsonOptions);
            await ProcessPriorityResponseAsync(priorityResponseMessage);
            
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received BSM priority response, id: {0}, vehicle: {1}", priorityResponseMessage?.PriorityResponse?.RequestId, priorityResponseMessage?.PriorityResponse?.VehicleId)));
        }
        else
        {
            _logger.LogWarning("Received BSM of unsupported format: {@json}", json);
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Warning, string.Format("Received BSM of unsupported format.")));
        }
    }

    private async Task ProcessBsmAsync(BsmMessage? bsmMessage)
    {
        // if (bsmMessage?.BsmMessageContent != null)
        // {
        //     foreach (var message in bsmMessage.BsmMessageContent)
        //     {
        //         var update = message.ToVehicleUpdate();
        //         await _vehiclePriorityService.UpdateVehicleAsync(update);
        //     }
        //
        //     _bsmCounter.Increment(bsmMessage.BsmMessageContent.Length);
        // }
    }

    private async Task ProcessSrmAsync(SrmMessage? srmMessage)
    {
        if (srmMessage?.SrmMessageContent != null)
        {
            _srmMessageRepository.Update(srmMessage);
            
            foreach (var message in srmMessage.SrmMessageContent)
            {
                var update = message.ToVehicleUpdate();
                await _vehiclePriorityService.UpdateVehicleAsync(update);
            }

            _bsmCounter.Increment(srmMessage.SrmMessageContent.Length);
        }
    }
    
    private async Task ProcessPriorityStatusAsync(PriorityStatusMessage? priorityStatusMessage)
    {
        if (priorityStatusMessage != null)
        {
            var statusList = priorityStatusMessage.PriorityStatus.Where(s => s.VehicleId != null).ToList();
            await _vehiclePriorityEdgePublisher.PublishPrsStatusAsync(priorityStatusMessage);

            _priorityStatusCounter.Increment(priorityStatusMessage.PriorityStatus?.Count() ?? 0);
        }
    }

    private async Task ProcessPriorityResponseAsync(PriorityResponseMessage? priorityResponseMessage)
    {
        if (priorityResponseMessage != null)
        {
            await _vehiclePriorityEdgePublisher.PublishPrsResponseAsync(priorityResponseMessage);

            _priorityResponseCounter.Increment();
        }
    }
}
