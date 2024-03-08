// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityPublisher : IVehiclePriorityPublisher
{
    private readonly IProducer<Guid, GenericJsonResponse> _configProducer;
    private readonly IMessageFactory<Guid, GenericJsonResponse> _configMessageFactory;
    private readonly IProducer<Guid, PriorityRequestMessage> _producer;
    private readonly IMessageFactory<Guid, PriorityRequestMessage> _messageFactory;
    private readonly ILogger<VehiclePriorityPublisher> _logger;
    private readonly string _configTopic;

    public VehiclePriorityPublisher(IConfiguration configuration, IProducer<Guid, GenericJsonResponse> configProducer, IMessageFactory<Guid, GenericJsonResponse> configMessageFactory, IProducer<Guid, PriorityRequestMessage> producer, IMessageFactory<Guid, PriorityRequestMessage> messageFactory, ILogger<VehiclePriorityPublisher> logger)
    {
        _configProducer = configProducer;
        _configMessageFactory = configMessageFactory;
        _producer = producer;
        _messageFactory = messageFactory;
        _logger = logger;
        
        _configTopic =  configuration["Topics:ConfigPriorityResponse"] ?? throw new NullReferenceException("Topics:ConfigPriorityResponse missing in config.");
    }
    
    public Task PublishVehicleUpdateAsync(VehicleUpdate update)
    {
        // var payload =
        //     _vehicleUpdateMessageFactory.Build(_ => _intersection, _intersection, update);
        // await _vehicleUpdateProducer.ProduceAsync("topic.OdeVehicleUpdate", payload);
        return Task.CompletedTask;
    }
    
    public async Task PublishEtaAsync(RouteStatus routeStatus)
    {
        var signalId = routeStatus.NextIntersection?.IntersectionId ?? Guid.Empty;
        if (signalId == Guid.Empty)
        {
            _logger.LogError("{RouteId} doesn't have the next intersection defined, skipping publishing of ETA",
                routeStatus.RouteId.ToString());
            return;
        }  
        var message = routeStatus.ToPriorityRequestMessage();
        var payload =
            _messageFactory.Build(routeStatus.Id, routeStatus.ToPriorityRequestMessage());
        await _producer.ProduceAsync("topic.PriorityRequest", payload);
    }

    public async Task PublishConfigAsync(PriorityRequestVehicleConfiguration config)
    {
        var json = JsonSerializer.Serialize(config, JsonPayloadSerializerOptions.Options);
        await _configProducer.ProduceAsync(_configTopic, _configMessageFactory.Build(Guid.Empty, new PriorityResponseConfigurationMessage(json)));
    }
}
