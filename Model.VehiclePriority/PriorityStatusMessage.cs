// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority;

public record PriorityStatusMessage(IEnumerable<PriorityStatus> PriorityStatus);

public record PriorityStatus(
    int PriorityRequestStatus,
    int RequestId,
    int StrategyNumber,
    int VehicleClassType,
    int VehicleClassLevel,
    string VehicleId
    );

public enum PriorityRequestStatus
{
    Unknown,
    IdleNotValid,
    ReadyQueued,
    ReadyOverridden,
    ActiveProcessing,
    ActiveCancel,
    ActiveOverride,
    ClosedCanceled,
    ReserviceError,
    ClosedTimeToLiveError,
    ClosedTimerError,
    ClosedStrategyError,
    ClosedCompleted,
    ActiveAdjustNotNeeded,
    ClosedFlash
}
