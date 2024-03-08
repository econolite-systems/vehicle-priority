// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Asn1J2735.J2735;
using Econolite.Ode.Domain.Asn1.J2735;
using Econolite.Ode.Domain.Configuration;
using Econolite.Ode.Domain.VehiclePriority.Extensions;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Entities.Types;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Econolite.Ode.Models.VehiclePriority.Config;
using Econolite.Ode.Repository.VehiclePriority;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityEdgePublisher : IVehiclePriorityEdgePublisher
{
    private readonly ILogger _logger;
    private readonly IMessageFactory<Guid, PriorityRequestMessage> _messageFactory;
    private readonly IProducer<Guid, PriorityStatusMessage> _priorityStatusProducer;
    private readonly IMessageFactory<Guid, PriorityStatusMessage> _priorityStatusMessageFactory;
    private readonly IProducer<Guid, PriorityResponseMessage> _priorityResponseProducer;
    private readonly IMessageFactory<Guid, PriorityResponseMessage> _priorityResponseMessageFactory;
    private readonly IProducer<Guid, RawSsmMessageJsonResponse> _genericJsonResponseProducer;
    private readonly IMessageFactory<Guid, RawSsmMessageJsonResponse> _genericJsonResponseMessageFactory;
    private readonly IOptionsMonitor<PriorityRequestUdpOptions> _options;
    private readonly IOptionsMonitor<IfmUdpOptions> _ifmUdpOptions;
    private readonly IAsn1J2735Service _asn1J2735Service;
    private readonly ISystemModellerService _systemModellerService;
    private readonly IProducer<Guid, VehicleUpdate> _vehicleUpdateProducer;
    private readonly IMessageFactory<Guid, VehicleUpdate> _vehicleUpdateMessageFactory;
    private readonly IProducer<Guid, PriorityRequestMessage> _producer;
    private readonly Guid _intersection;
    private readonly string _topic;
    private readonly string _topicPrsStatus;
    private readonly string _topicPrsRequest;
    private readonly string _topicPrsResponse;
    private readonly string _topicOdeSsm;
    private readonly ISrmMessageRepository _srmMessageRepository;

    public VehiclePriorityEdgePublisher(IConfiguration configuration,
        IProducer<Guid, VehicleUpdate> vehicleUpdateProducer,
        IMessageFactory<Guid, VehicleUpdate> vehicleUpdateMessageFactory,
        IProducer<Guid, PriorityRequestMessage> producer,
        IMessageFactory<Guid, PriorityRequestMessage> messageFactory,
        IProducer<Guid, PriorityStatusMessage> priorityStatusProducer,
        IMessageFactory<Guid, PriorityStatusMessage> priorityStatusMessageFactory,
        IProducer<Guid, PriorityResponseMessage> priorityResponseProducer,
        IMessageFactory<Guid, PriorityResponseMessage> priorityResponseMessageFactory,
        IProducer<Guid, RawSsmMessageJsonResponse> genericJsonResponseProducer,
        IMessageFactory<Guid, RawSsmMessageJsonResponse> genericJsonResponseMessageFactory,
        IOptionsMonitor<PriorityRequestUdpOptions> options,
        IOptionsMonitor<IfmUdpOptions> ifmUdpOptions,
        IAsn1J2735Service asn1J2735Service,
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (loggerFactory is null)
        {
            throw new ArgumentNullException(nameof(loggerFactory));
        }

        if (!Guid.TryParse(configuration["Intersection"], out _intersection))
        {
            _intersection = Guid.Empty;
        };
        _topic = configuration[Consts.TOPIC_ODE_VEHICLE_UPDATE] ?? Consts.TOPIC_ODE_VEHICLE_UPDATE_DEFAULT;
        _topicPrsStatus = configuration[Consts.TOPIC_ODE_VEHICLE_STATUS] ?? Consts.TOPIC_ODE_VEHICLE_STATUS_DEFAULT;
        _topicPrsRequest = configuration[Consts.TOPIC_ODE_VEHICLE_REQUEST] ?? Consts.TOPIC_ODE_VEHICLE_REQUEST_DEFAULT;
        _topicPrsResponse = configuration[Consts.TOPIC_ODE_VEHICLE_RESPONSE] ?? Consts.TOPIC_ODE_VEHICLE_RESPONSE_DEFAULT;
        _topicOdeSsm = configuration[Consts.TOPIC_ODE_SSM] ?? Consts.TOPIC_ODE_SSM_DEFAULT;
        _vehicleUpdateProducer = vehicleUpdateProducer ?? throw new ArgumentNullException(nameof(vehicleUpdateProducer));
        _vehicleUpdateMessageFactory = vehicleUpdateMessageFactory ?? throw new ArgumentNullException(nameof(vehicleUpdateMessageFactory));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
        _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
        _priorityStatusProducer = priorityStatusProducer ?? throw new ArgumentNullException(nameof(priorityStatusProducer));
        _priorityStatusMessageFactory = priorityStatusMessageFactory ?? throw new ArgumentNullException(nameof(priorityStatusMessageFactory));
        _priorityResponseProducer = priorityResponseProducer ?? throw new ArgumentNullException(nameof(priorityResponseProducer));
        _priorityResponseMessageFactory = priorityResponseMessageFactory ?? throw new ArgumentNullException(nameof(priorityResponseMessageFactory));
        _genericJsonResponseProducer = genericJsonResponseProducer;
        _genericJsonResponseMessageFactory = genericJsonResponseMessageFactory;
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _ifmUdpOptions = ifmUdpOptions;
        _asn1J2735Service = asn1J2735Service;
        using var scope = serviceProvider.CreateScope();
        _systemModellerService = scope.ServiceProvider.GetRequiredService<ISystemModellerService>();
        _srmMessageRepository = scope.ServiceProvider.GetRequiredService<ISrmMessageRepository>();
        _logger = loggerFactory.CreateLogger(GetType().Name);
    }
    
    public async Task PublishSrmStatusAsync(PriorityStatusMessage update)
    {
        
        _logger.LogTrace("Publishing PRS status {@}", update);
        var payload = _priorityStatusMessageFactory.Build(_intersection, update);
        await _priorityStatusProducer.ProduceAsync(_topicPrsStatus, payload);
    }
    
    public async Task PublishPrsStatusAsync(PriorityStatusMessage update)
    {
        var srmMessages = await _srmMessageRepository.GetAllAsync();
        var messages = srmMessages.Where(m => update.PriorityStatus.Any(s => s.RequestId == m.SrmMessageContent.FirstOrDefault()?.Srm.Requests.FirstOrDefault()?.RequestId)).ToArray();
        if (messages.Any())
        {
            var ssm = messages.ToSsm(update.PriorityStatus);
            await PublishSsmAsync(ssm);
        }
        _logger.LogTrace("Publishing PRS status {@}", update);
        var payload = _priorityStatusMessageFactory.Build(_intersection, update);
        await _priorityStatusProducer.ProduceAsync(_topicPrsStatus, payload);
    }
    
    public async Task PublishPrsResponseAsync(PriorityResponseMessage update)
    {
        _logger.LogInformation("Handling PRS status response {@}", update);
        var payload = _priorityResponseMessageFactory.Build(_intersection, update);
        await _priorityResponseProducer.ProduceAsync(_topicPrsResponse, payload);
    }
    
    public async Task PublishVehicleUpdateAsync(VehicleUpdate update)
    {
        _logger.LogDebug("Publishing vehicle update {@}", update);
        var payload = _vehicleUpdateMessageFactory.Build(_intersection, update);
        await _vehicleUpdateProducer.ProduceAsync(_topic, payload);
    }
    
    public async Task PublishEtaAsync(RouteStatus routeStatus)
    {
        var priorityRequestMessage = routeStatus.ToPriorityRequestMessage();

        if (!IsValidMessage(priorityRequestMessage))
        {
            _logger.LogWarning("RouteStatus not valid: {@}", priorityRequestMessage);
            return;
        }

        using var udpClient = new UdpClient();
        var json = JsonSerializer.Serialize(priorityRequestMessage, JsonPayloadSerializerOptions.Options);
        var package = Encoding.UTF8.GetBytes(json);
        var ip = new IPEndPoint(IPAddress.Parse(_options.CurrentValue.Host), _options.CurrentValue.Port);
        _logger.LogInformation("Sending ETA {@}", routeStatus);
        await Task.WhenAll(
            udpClient.SendAsync(package, package.Length, ip),
            _producer.ProduceAsync(_topicPrsRequest, _messageFactory.Build(routeStatus.Id, routeStatus.ToPriorityRequestMessage())));
    }

    public async Task PublishSsmAsync(SignalStatusMessage signalStatus)
    {
        var nodes = await _systemModellerService.GetAllAsync();
        var rsu = nodes.FirstOrDefault(n => n.Type.Id == RsuTypeId.Id);
        if (rsu == null)
        {
            _logger.LogWarning("No RSU found");
            return;
        }
        var messageFrame = new MessageFrame
        {
            Value = signalStatus
        };
        var encodeSsm = _asn1J2735Service.EncodeSsm(messageFrame);
        using var udpClient = new UdpClient();
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var message = new RawSsmMessageJsonResponse()
        {
            SsmMessageContent = new []
            {
                new RawMessageContent()
                {
                    Metadata = new RawMessageMetadata()
                    {
                        OriginRsu = rsu.IPAddress,
                        UtcTimestamp = timestamp
                    },
                    Payload = encodeSsm
                }
            }
        };
        var ssmIfm = $"Version=0.7\nType=SSM\nPSID=0xe0000015\nPriority=7\nTxMode=ALT\nTxChannel=183\nTxInterval=0\nDeliveryStart=\nDeliveryStop=\nSignature=False\nEncryption=False\nPayload={encodeSsm}";
        var package = Encoding.UTF8.GetBytes(ssmIfm);
        var ip = new IPEndPoint(IPAddress.Parse(rsu.IPAddress), _ifmUdpOptions.CurrentValue.Port);
        _logger.LogDebug("Publishing SSM {@}", signalStatus);
        await Task.WhenAll(
            udpClient.SendAsync(package, package.Length, ip),
            _genericJsonResponseProducer.ProduceAsync(_topicOdeSsm, _genericJsonResponseMessageFactory.Build(Guid.Empty, message))
        );
    }

    private bool IsValidMessage(PriorityRequestMessage message) => message.Request.StrategyNumber >= 1;
}

public class RawSsmMessageJsonResponse
{
    [JsonPropertyName("SsmMessageContent")]
    public IEnumerable<RawMessageContent> SsmMessageContent { get; set; } = Array.Empty<RawMessageContent>();
}

public class RawMessageContent
{
    [JsonPropertyName("metadata")]
    public RawMessageMetadata Metadata { get; set; } = new RawMessageMetadata();
    [JsonPropertyName("payload")]
    public string Payload { get; set; } = string.Empty;
}

public class RawMessageMetadata
{
    [JsonPropertyName("utctimestamp")]
    public string UtcTimestamp { get; set; } = string.Empty;
    [JsonPropertyName("originRsu")]
    public string OriginRsu { get; set; } = string.Empty;
}