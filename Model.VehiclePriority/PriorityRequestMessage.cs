// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Diagnostics;

namespace Econolite.Ode.Models.VehiclePriority;

public enum PriorityRequestType
{
    Initial,
    Update,
    Cancel
}

public record PriorityRequestMessage(PriorityRequestType RequestType, PRequest Request);

public record PRequest (
    byte RequestId,
    string VehicleId,
    byte VehicleClassType,
    byte VehicleClassLevel,
    byte StrategyNumber,
    ushort? TimeOfServiceDesired,
    ushort? TimeOfEstimatedDeparture);

public static class PriorityRequestExtension
{
    private static int ALLOW_FOR_LATENCY_MS_IN_SET_REQUEST_MEHOD = 500;
    private static int ETD_SECONDS = 5;

    public static PriorityRequestMessage ToPriorityRequestMessage(this RouteStatus status)
    {
        return new PriorityRequestMessage(status.ToPriorityRequestType(), status.ToRequest());
    }

    private static PriorityRequestType ToPriorityRequestType(this RouteStatus status)
    {
        return status.IsInitial ? PriorityRequestType.Initial :
            status.Completed ? PriorityRequestType.Cancel :
            PriorityRequestType.Update;
    }

    private static PRequest ToRequest(this RouteStatus status)
    {
        return new PRequest(
            (byte) status.RequestId,
            status.VehicleId ?? String.Empty,
            (byte) (status.VehicleTypePriority ?? 10),
            (byte) (status.DesiredClassLevel ?? 10 ),
            (byte) (status.NextIntersection?.Plan ?? -1),
            status.Completed ? null : (ushort) status.EtaInSeconds, 
            status.Completed ? null : (ushort) (status.EtaInSeconds + ETD_SECONDS));
    }

    public static TimeSpan AnticipatedTimeInSecondsFromNow(DateTime anticipatedTime)
    {
        return anticipatedTime - DateTime.Now;
    }

    public static void AddSecondsFromNowToOERArray(int arrayOffset, TimeSpan anticipatedTimeInSecondsFromNow, ref byte[] oerData)
    {
        var secondsFromNowDbl = anticipatedTimeInSecondsFromNow.TotalSeconds;

        // zero not legal.
        if (secondsFromNowDbl < 1.0)
        {
            secondsFromNowDbl = 1.0;
        }

        ushort secondsFromNow = (ushort)(secondsFromNowDbl);
        var estimatedSecondsFromNowByteArray = BitConverter.GetBytes(secondsFromNow);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(estimatedSecondsFromNowByteArray);

        Array.Copy(estimatedSecondsFromNowByteArray, 0, oerData, arrayOffset, 2);
    }

    public static void AddSecondsFromNowToOERArray(int arrayOffset, DateTime anticipatedTime, ref byte[] oerData)
    {
        TimeSpan anticipatedTimeInSecondsFromNow = anticipatedTime - DateTime.Now;
        AddSecondsFromNowToOERArray(arrayOffset, anticipatedTimeInSecondsFromNow, ref oerData);
    }
    
    private static bool AreAnticipatedTimesValid(DateTime anticipatedArrival, DateTime anticipatedDeparture, byte requestID)
    {
        bool result = true;
        
        if (anticipatedArrival != DateTime.MinValue)
        {
            if (anticipatedArrival.AddMilliseconds(ALLOW_FOR_LATENCY_MS_IN_SET_REQUEST_MEHOD) <= DateTime.Now)
            {
                result = false;
            }

            // Departure isn't used yet at the controller so I don't care about this condition right now.
            if (anticipatedDeparture < anticipatedArrival)
            {
                result = false;
            }
        }

        return result;
    }
}
