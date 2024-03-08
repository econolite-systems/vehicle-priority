// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm;

public class RealTimeData
{
    public RealTimeData(int queueLength, float averageSpeedAdjustment)
    {
        QueueLength = queueLength;
        AverageSpeedAdjustment = averageSpeedAdjustment;
    }

    public int QueueLength { get; set; } //Number of regular sized vehicles waiting
    public float AverageSpeedAdjustment { get; set; } //Rate of change to average speed based on current conditions

}
