// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.VehiclePriority.Status;
using Microsoft.AspNetCore.SignalR;

namespace Econolite.Ode.Domain.VehiclePriority;

public class VehiclePriorityVehicleStatusHub : Hub
{
    public async Task SendSignalUpdateAsync(RoutePriorityStatus update) {
        if (Clients != null)
        {
            await Clients.All.SendAsync("signalStatusUpdate", JsonSerializer.Serialize(update, JsonPayloadSerializerOptions.Options));
        }
    }
    
    public async Task SendUpdateAsync(VehicleLocationStatus update) {
        if (Clients != null)
        {
            await Clients.All.SendAsync("vehicleLocationStatus", JsonSerializer.Serialize(update, JsonPayloadSerializerOptions.Options));
        }
    }
}
