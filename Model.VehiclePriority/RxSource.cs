// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority;

public enum RxSource {
    RSU,
    SAT, // XM satellite
    RV,  // for BSM rx
    SNMP,// for SRM payload from back-end/ODE
    NA,  // Not applicable (for example, Distress Notification or Driver Alert 
    UNKNOWN
}
