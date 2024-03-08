// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Config;

public class PriorityRequestUdpOptions
{
    public const string Section = "PriorityRequestUdp";
    
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 9097;
}
