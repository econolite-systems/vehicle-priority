// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Authorization;
using Econolite.Ode.Domain.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Microsoft.AspNetCore.Mvc;

namespace Econolite.Ode.Api.VehiclePriority.Controllers;

/// <summary>
/// The api controller that manages the vehicle priority.
/// </summary>
[ApiController]
[Route("scp-vehicle-class")]
[AuthorizeOde(MoundRoadRole.ReadOnly)]
public class PriorityVehicleClassController : ControllerBase
{
    private readonly IVehiclePriorityService _vehiclePriorityService;
    private readonly ILogger<PriorityVehicleClassController> _logger;

    /// <summary>
    /// The constructor api controller that manages the vehicle priority.
    /// </summary>
    /// <param name="vehiclePriorityService"></param>
    /// <param name="logger"></param>
    public PriorityVehicleClassController(IVehiclePriorityService vehiclePriorityService, ILogger<PriorityVehicleClassController> logger)
    {
        _vehiclePriorityService = vehiclePriorityService;
        _logger = logger;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(PriorityRequestVehicleConfiguration))]
    public async Task<ActionResult<PriorityRequestVehicleConfiguration>> IndexAsync()
    {
        var configs = await _vehiclePriorityService.GetAllPriorityRequestVehicleClassesAsync();
        return Ok(configs);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(200, Type = typeof(bool))]
    [AuthorizeOde(MoundRoadRole.Contributor)]
    public async Task<IActionResult> PutAsync([FromBody] PriorityRequestVehicleConfiguration value)
    {
        _logger.LogDebug("Updating {@}", value);

        if (value == null) return BadRequest();
        try
        {
            var updated = await _vehiclePriorityService.UpdatePriorityRequestVehicleClassesAsync(value);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to update {@}", value);
            return StatusCode(500, ex.Message);
        }
    }
}
