// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.Logging;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehicleUpdateSmoothing
{
    private readonly IVehiclePriorityService _service;
    private readonly ILogger<VehicleUpdateSmoothing> _logger;

    public VehicleUpdateSmoothing(IVehiclePriorityService service, ILogger<VehicleUpdateSmoothing> logger)
    {
        _service = service;
        _logger = logger;
    }

}
