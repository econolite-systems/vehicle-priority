// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Auditing;
using Econolite.Ode.Auditing.Extensions;
using Econolite.Ode.Authorization;
using Econolite.Ode.Domain.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Microsoft.AspNetCore.Mvc;

namespace Econolite.Ode.Api.VehiclePriority.Controllers;

/// <summary>
/// The api controller that manages the vehicle priority.
/// </summary>
[ApiController]
[Route("scp")]
[AuthorizeOde(MoundRoadRole.Contributor)]
public class VehiclePriorityController : ControllerBase
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly ILogger<VehiclePriorityController> _logger;
    private readonly IAuditCrudScopeFactory _auditCrudScopeFactory;
    private readonly string _auditEventType;

    /// <summary>
    /// The constructor api controller that manages the vehicle priority.
    /// </summary>
    /// <param name="vehiclePriorityService"></param>
    /// <param name="logger"></param>
    /// <param name="auditCrudScopeFactory"></param>
    public VehiclePriorityController(IVehiclePriorityService vehiclePriorityService, ILogger<VehiclePriorityController> logger, IAuditCrudScopeFactory auditCrudScopeFactory)
    {
        _vehiclePriorityService = vehiclePriorityService;
        _logger = logger;
        _auditCrudScopeFactory = auditCrudScopeFactory;
        _auditEventType = SupportedAuditEventTypes.AuditEventTypes[AuditEventType.VehiclePriority].Event;
    }

    /// <summary>
    ///     Should be called by a connected remote system at least once every 10 seconds so that system
    ///     components can be monitored as online or offline.
    /// </summary>
    /// <param name="sourceId">Agreed upon system ID</param>
    /// <returns></returns>
    [HttpPut("registerheartbeat")]
    public async Task<bool> RegisterHeartbeatAsync(string sourceId)
    {
        return await _vehiclePriorityService.RegisterHeartbeatAsync(sourceId);
    }

    /// <summary>
    ///     Activate a priority request for a vehicle.
    /// </summary>
    /// <returns>Request ID or null of request failed to activate</returns>
    [HttpPost("requestpriority")]
    public async Task<string?> RequestPriorityAsync(PriorityRequest request)
    {
        _logger.LogDebug("Adding {@}", request);
        var scope = _auditCrudScopeFactory.CreateAddAsync(_auditEventType, () => request);
        await using (await scope)
        {
            _logger.LogInformation("Priority request received and activated: {RequestRequestId}", request.RequestId);
            var result = await _vehiclePriorityService.RequestPriorityAsync(request);
            return result?.RequestId;
        }
    }

    /// <summary>
    ///     Cancel a currently active priority request.
    /// </summary>
    /// <param name="requestId">ID of the request</param>
    /// <returns>true if canceled or false if request not found or not active</returns>
    [HttpPut("cancelpriorityrequest")]
    public async Task<bool> CancelPriorityRequestAsync(string requestId)
    {
        _logger.LogDebug("Updating {@}", requestId);
        var scope = _auditCrudScopeFactory.CreateUpdateAsync(_auditEventType, () => requestId);
        await using (await scope)
        {
            _logger.LogInformation("Priority request canceled: {RequestId}", requestId);
            return await _vehiclePriorityService.CancelPriorityRequestAsync(requestId);
        }
    }

    /// <summary>
    ///     Update vehicle position.
    /// </summary>
    /// <param name="vehicleid">The id.</param>
    /// <param name="latitude">The lat.</param>
    /// <param name="longitude">The lon.</param>
    /// <param name="speedmph">The speed in mph.</param>
    /// <param name="heading">The heading in degrees.</param>
    [HttpPut("updatevehicleposition")]
    public async Task UpdateVehiclePositionAsync(string vehicleid, double latitude, double longitude, double speedmph,
        string heading)
    {
        var update = new VehicleUpdate
        {
            RouteId = "0",
            VehicleId = vehicleid,
            VehicleName = vehicleid,
            VehicleLatitude = latitude.ToString(),
            VehicleLongitude = longitude.ToString(),
            TravelDirection = int.Parse(heading),
            TravelSpeed = speedmph
        };

        _logger.LogDebug("Updating {@}", update);
        var scope = _auditCrudScopeFactory.CreateUpdateAsync(_auditEventType, () => update);
        await using (await scope)
        {
            _logger.LogInformation("Processing vehicle update: {UpdateVehicleId}", update.VehicleId);
            await _vehiclePriorityService.UpdateVehicleAsync(update);
        }
    }

    /// <summary>
    ///     Update the vehicle.
    /// </summary>
    /// <param name="update">The vehicle update.</param>
    [HttpPut("updatevehicle")]
    public async Task UpdateVehicleAsync([FromBody] VehicleUpdate update)
    {
        _logger.LogDebug("Updating {@}", update);
        var scope = _auditCrudScopeFactory.CreateUpdateAsync(_auditEventType, () => update);
        await using (await scope)
        {
            await _vehiclePriorityService.UpdateVehicleAsync(update);
            _logger.LogInformation("Processing vehicle update: {UpdateVehicleId}", update.VehicleId);
        }
    }

    /// <summary>
    ///     Add a new route to the system.
    /// </summary>
    /// <param name="route"></param>
    /// <returns>ID of the new route</returns>
    [HttpPost("registerroute")]
    public async Task<string?> RegisterRouteAsync([FromBody] RouteUpdate route)
    {
        _logger.LogDebug("Adding {@}", route);
        var scope = _auditCrudScopeFactory.CreateAddAsync(_auditEventType, () => route);
        await using (await scope)
        {
            var routeId = Guid.NewGuid().ToString();
            route.RouteId = routeId;
            var result = await _vehiclePriorityService.RegisterRouteAsync(route);
            _logger.LogInformation("Registed new route, ID: {RouteId}", routeId);
            return result;
        }
    }

    /// <summary>
    ///     Reset an already registered route.
    /// </summary>
    [HttpPut("updateroute")]
    public async Task UpdateRouteAsync([FromBody] RouteUpdate route)
    {
        _logger.LogDebug("Updating {@}", route);
        var scope = _auditCrudScopeFactory.CreateUpdateAsync(_auditEventType, () => route);
        await using (await scope)
        {
            await _vehiclePriorityService.UpdateRouteAsync(route);
            _logger.LogInformation("Updated route with ID: {RouteRouteId}", route.RouteId);
        }
    }
}
