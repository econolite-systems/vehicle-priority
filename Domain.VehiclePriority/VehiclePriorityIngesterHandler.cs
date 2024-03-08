// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Client.VehiclePriority.Model.J2735;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Monitoring.Events;
using Econolite.Ode.Monitoring.Events.Extensions;
using Econolite.Ode.Monitoring.Metrics;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityIngesterHandler
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly IVehiclePriorityEdgePublisher _vehiclePriorityEdgePublisher;
    private readonly ILogger<VehiclePriorityIngesterHandler> _logger;
    private readonly UserEventFactory _userEventFactory;
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

    public VehiclePriorityIngesterHandler(IVehiclePriorityService vehiclePriorityService, IVehiclePriorityEdgePublisher vehiclePriorityEdgePublisher, ILogger<VehiclePriorityIngesterHandler> logger, IMetricsFactory metricsFactory, UserEventFactory userEventFactory)
    {
        _vehiclePriorityService = vehiclePriorityService;
        _vehiclePriorityEdgePublisher = vehiclePriorityEdgePublisher;
        _logger = logger;
        _userEventFactory = userEventFactory;

        _loopCounter = metricsFactory.GetMetricsCounter("Edge Bsm Worker");
        _bsmCounter = metricsFactory.GetMetricsCounter("Edge Bsm Count");
        _priorityStatusCounter = metricsFactory.GetMetricsCounter("Edge Priority Status");
        _priorityResponseCounter = metricsFactory.GetMetricsCounter("Edge Priority Response");
    }
    
    public async Task ProcessAsync(UdpReceiveResult result)
    {
        var json = Encoding.ASCII.GetString(result.Buffer);

        if (json.Contains("srm"))
        {
            var srmMessage = JsonSerializer.Deserialize<SrmMessage>(json, _jsonOptions);
            await ProcessSrmAsync(srmMessage);
            
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received SRM messages: {0}", srmMessage?.SrmMessageContent?.Length)));
        }
        else if (json.Contains("priorityStatus"))
        {
            var priorityStatusMessage = JsonSerializer.Deserialize<PriorityStatusMessage>(json, _jsonOptions);
            await ProcessPriorityStatusAsync(priorityStatusMessage);

            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received priority status: {0}", priorityStatusMessage?.PriorityStatus?.Count())));
        }
        else if (json.Contains("priorityResponse"))
        {
            var priorityResponseMessage = JsonSerializer.Deserialize<PriorityResponseMessage>(json, _jsonOptions);
            await ProcessPriorityResponseAsync(priorityResponseMessage);
            
            _logger.ExposeUserEvent(_userEventFactory.BuildUserEvent(EventLevel.Debug, string.Format("Received priority response, id: {0}, vehicle: {1}", priorityResponseMessage?.PriorityResponse?.RequestId, priorityResponseMessage?.PriorityResponse?.VehicleId)));
        }
    }

    private async Task ProcessSrmAsync(SrmMessage? srmMessage)
    {
        if (srmMessage?.SrmMessageContent != null)
        {
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
