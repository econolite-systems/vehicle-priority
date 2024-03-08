// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm;

public class CurrentVehicleData
{
    /// <summary>
    /// Distance out is in feet from approach.
    /// </summary>
    public int DistanceOut { get; set; }
    
    /// <summary>
    /// Current speed in mph.
    /// </summary>
    public float CurrentSpeed { get; set; }
    public bool WillStopAtStopLocation { get; set; }
    
    public CurrentVehicleData(int distanceOut, float currentSpeed, bool willStopAtStopLocation)
    {
        DistanceOut = distanceOut;
        CurrentSpeed = currentSpeed;
        WillStopAtStopLocation = willStopAtStopLocation;
    }
}
