// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Models.VehiclePriority.Algorithm;

public class PriorityRequest
{
    public CurrentVehicleData VehicleData { get; set; } = new CurrentVehicleData(0, 0, false);
    public RealTimeData RealTimeData { get; set; } = new RealTimeData(0, 0);
    public GeneralRoadwayData RoadwayData { get; set; } = new GeneralRoadwayData(0, false, 0);
    public HistoricalAnalysisData HistoricalData { get; set; } =
        new HistoricalAnalysisData(new HistoricalAnalysisData.VarianceResultWithInfo());
}
