// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Dynamic;

namespace Econolite.Ode.Models.VehiclePriority;

public class Controller
{
    public Guid IntersectionId { get; set; }
    
    public int Plan { get; set; }
}
