// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Econolite.Ode.Models.VehiclePriority.Algorithm;
//
// namespace ETA_Predictor
// {
//     public class ETA_Predictor
//     {
//         const double VARIANCE_REDUCTION = .5f; //Percentage reduction in variance required to create new split
//         const double TIME_DIFF = 4;
//         public static List<EtaResult> GetETA(List<TripPointData> allTrips)
//         {
//             List<EtaResult> result = new List<EtaResult>();
//
//             var tripGroup = GroupTrips(allTrips);
//             result.Add(GetEtaResultFromTripGroups(tripGroup));
//             
//             return result;
//         }
//
//         private static EtaResult GetEtaResultFromTripGroups(List<TripGroup> tripGroup)
//         {
//             EtaResult result = new EtaResult();
//
//             if (tripGroup.Any())
//             {
//                 var fullGroup = tripGroup.First();
//                 var fullGroupVarianceResult = GetVariance(fullGroup.GroupedPoints.Select(x => x.ArrivalTimeFromHere).ToList());
//                 double previousDeviation = fullGroupVarianceResult.Deviation;
//                 int lastMaxDistance = tripGroup.First().MaxDistance;
//                 bool firstEntry = true;
//                 foreach (var group in tripGroup)
//                 {
//                     var splitResult = GetSplitResult(group.GroupedPoints);
//                     if (splitResult.LowerVariance.Deviation + splitResult.UpperVariance.Deviation < VARIANCE_REDUCTION * previousDeviation)
//                     {
//                         previousDeviation = splitResult.LowerVariance.Deviation + splitResult.UpperVariance.Deviation;
//                         result.ResultsList.AddRange(CreateVarianceResultFromSplitResult(splitResult, group.MinDistance, group.MaxDistance, ResultType.Normal));
//                     }
//                     else
//                     {
//                         if (firstEntry)
//                         {
//                             VarianceResultWithInfo firstVarResult = new VarianceResultWithInfo();
//                             firstVarResult.LinearCorrelation = false;
//                             firstVarResult.MaxDistance = group.MaxDistance;
//                             firstVarResult.MinDistance = group.MinDistance;
//                             firstVarResult.MinSpeed = -1;
//                             firstVarResult.MaxSpeed = 999;
//                             firstVarResult.ResultType = ResultType.Normal;
//                             firstVarResult.VarianceResult = fullGroupVarianceResult;
//                             result.ResultsList.Add(firstVarResult);
//                             firstEntry = false;
//                         }
//                     }
//
//                 }
//
//                 //Test split groups for linear correlation
//             }
//
//             return result;
//         }
//
//         private static List<VarianceResultWithInfo> CreateVarianceResultFromSplitResult(SplitResult splitResult, int minDist, int maxDist, ResultType type)
//         {
//             List<VarianceResultWithInfo> result = new List<VarianceResultWithInfo>();
//
//             VarianceResultWithInfo lowerResult = new VarianceResultWithInfo();
//             lowerResult.LinearCorrelation = false;
//             lowerResult.MaxDistance = maxDist;
//             lowerResult.MinDistance = minDist;
//             lowerResult.MinSpeed = -1;
//             lowerResult.MaxSpeed = splitResult.SplitSpeed;
//             lowerResult.ResultType = type;
//             lowerResult.VarianceResult = splitResult.LowerVariance;
//             result.Add(lowerResult);
//
//             VarianceResultWithInfo upperResult = new VarianceResultWithInfo();
//             upperResult.LinearCorrelation = false;
//             upperResult.MaxDistance = maxDist;
//             upperResult.MinDistance = minDist;
//             upperResult.MinSpeed = -1;
//             upperResult.MaxSpeed = splitResult.SplitSpeed;
//             upperResult.ResultType = type;
//             upperResult.VarianceResult = splitResult.LowerVariance;
//             result.Add(upperResult);
//
//             return result;
//         }
//
//         //Return trips in order from highest distance to lowest distance
//         private static List<TripGroup> GroupTrips(List<TripPointData> tripsToGroup)
//         {
//             List<TripGroup> result = new List<TripGroup>();
//
//             return result;
//         }
//
//         private static SplitResult GetSplitResult(List<TripPoint> observedPoints)
//         {
//             SplitResult result = new SplitResult();
//             var groupedPoints = observedPoints.GroupBy(x => x.Speed);
//             double bestDifference = double.MaxValue;
//             List<int> lowTimes = new List<int>(), highTimes = new List<int>(observedPoints.Select(x => x.ArrivalTimeFromHere));
//             VarianceResult bestLowVariance = new VarianceResult(), bestHighVariance = new VarianceResult();
//
//             foreach (var group in groupedPoints)
//             {
//                 foreach (var item in group)
//                 {
//                     highTimes.Remove(item.ArrivalTimeFromHere);
//                     lowTimes.Add(item.ArrivalTimeFromHere);
//                 }
//                 var highVariance = GetVariance(highTimes);
//                 var lowVariance = GetVariance(lowTimes);
//
//                 double differenceMeasure = lowVariance.Deviation + highVariance.Deviation;
//                 if (differenceMeasure < bestDifference)
//                 {
//                     result.SplitSpeed = group.Key;
//                     result.LowerVariance = lowVariance;
//                     result.UpperVariance = highVariance;
//                 }
//             }
//
//             return result;
//         }
//
//         private static VarianceResult GetVariance(List<int> arrivalTimeObservations)
//         {
//             VarianceResult result = new VarianceResult();
//             result.Mean = arrivalTimeObservations.Average();
//             result.Max = arrivalTimeObservations.Max();
//             result.Min = arrivalTimeObservations.Min();
//             result.ObsCount = arrivalTimeObservations.Count;
//             result.Deviation = arrivalTimeObservations.Sum(x => (x - result.Mean) * (x - result.Mean));
//             return result;
//         }
//     }
// }

