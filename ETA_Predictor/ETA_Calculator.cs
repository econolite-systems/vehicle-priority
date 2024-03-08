// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Models.VehiclePriority.Algorithm;

namespace ETA_Predictor
{
    public class EtaCalculator
    {
        public static EtaResult GetEta(CurrentVehicleData currentVehicleData, RealTimeData realTimeData, HistoricalAnalysisData historicalAnalysisData, GeneralRoadwayData generalRoadwayData)
        {
            EtaResult result = new EtaResult();
            result.Variance = 0;
            result.VarianceTooHigh = HasNotStopped(currentVehicleData, generalRoadwayData);

            var expectedSpeed = generalRoadwayData.ExpectedSpeed * realTimeData.AverageSpeedAdjustment;
            var stopTime = CalculateStopTime(currentVehicleData, generalRoadwayData); //Time expected to be lost due to a bus stop or other planned stop
            var shiftForQueue = QueueShiftTime(realTimeData);
            var baseEta = CalculateTimeFromSpeedAndDistance(expectedSpeed, currentVehicleData.DistanceOut);

            result.Eta = baseEta + stopTime - shiftForQueue;
            if (result.Eta < 1)
                result.Eta = 1;

            return result;
        }

        //Time (seconds) expected to be lost due to a bus stop or other planned stop
        //TODO: Pass in data about expected stop length
        private static int CalculateStopTime(CurrentVehicleData currentVehicleData, GeneralRoadwayData generalRoadwayData)
        {
            if (!currentVehicleData.WillStopAtStopLocation)
                return 0;

            float slowDownTime = 5;
            float speedUpTime = 5;
            //TODO: Update times based on speed of road
            float expectedStopLength = 25;

            return (int) (slowDownTime + speedUpTime + expectedStopLength);
        }

        //TODO: For distances close to stop distance, check previous speeds to see if already stopped
        private static bool HasNotStopped(CurrentVehicleData currentVehicleData, GeneralRoadwayData generalRoadwayData)
        {
            return currentVehicleData.WillStopAtStopLocation && currentVehicleData.DistanceOut > generalRoadwayData.StopDistance;
        }

        //Amount of time to shift ETA earlier to account for queue clearance time
        //TODO: May want to account for phase/min green, for example may not want to shift as early for left turns
        private static int QueueShiftTime(RealTimeData realTimeData)
        {
            const float timePerVehicle = 2; //TODO: Update using queue clearance model
            return (int)(timePerVehicle * realTimeData.QueueLength);
        }

        private static int CalculateTimeFromSpeedAndDistance(float speed, int distance)
        {
            var ftPerSec = speed * 5280 / 3600;
            return (int) (distance / ftPerSec);
        }
    }
}

