// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm;

public class GeneralRoadwayData
{
    /// <summary>
    /// Expected speed is in mph.
    /// </summary>
    public float ExpectedSpeed { get; set; }
    public bool HasStop { get; set; }
    
    /// <summary>
    /// Stop distance is in feet.
    /// </summary>
    public int StopDistance { get; set; }
    
    public GeneralRoadwayData(float expectedSpeed, bool hasStop, int stopDistance)
    {
        ExpectedSpeed = expectedSpeed;
        HasStop = hasStop;
        StopDistance = stopDistance;
    }
}
