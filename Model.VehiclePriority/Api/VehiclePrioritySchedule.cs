// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority.Api;

public record VehiclePrioritySchedule();

public record VehicleEnableSchedule(string VehicleId, IEnumerable<ScheduleRange> Schedule);
public record ScheduleRange(DateTime start, DateTime end);
