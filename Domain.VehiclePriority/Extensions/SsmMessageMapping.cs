// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text;
using Econolite.Asn1J2735.J2735;
using Econolite.Ode.Domain.Asn1.J2735.Mapping;
using Econolite.Client.VehiclePriority.Model.J2735;
using Econolite.Ode.Models.VehiclePriority;
using PriorityRequestType = Econolite.Asn1J2735.J2735.PriorityRequestType;

namespace Econolite.Ode.Domain.VehiclePriority.Extensions;

public static class SsmMessageMapping
{
    public static SignalStatusMessage ToSsm(this IEnumerable<SrmMessage> srmMessages, IEnumerable<PriorityStatus> status)
    {
        var signalRequestMessages = srmMessages.Select(MapToSignalRequestMessage);
        var priorityStatuses = status.ToDictionary(s => s.RequestId, s => s.ToStatus());
        return signalRequestMessages.ToSsm(priorityStatuses);
    }
    
    private static PrioritizationResponseStatus ToStatus(this PriorityStatus priorityRequestStatus)
    {
        switch ((PriorityRequestStatus)priorityRequestStatus.PriorityRequestStatus)
        {
            case PriorityRequestStatus.Unknown:
                return PrioritizationResponseStatus.Unknown;
            case PriorityRequestStatus.ReadyQueued:
                return PrioritizationResponseStatus.Processing;
            case PriorityRequestStatus.ActiveProcessing:
                return PrioritizationResponseStatus.Processing;
            case PriorityRequestStatus.ReserviceError:
                return PrioritizationResponseStatus.ReserviceLocked;
            case PriorityRequestStatus.ClosedStrategyError:
            case PriorityRequestStatus.ClosedTimerError:
            case PriorityRequestStatus.ClosedTimeToLiveError:
            case PriorityRequestStatus.IdleNotValid:
                return PrioritizationResponseStatus.Rejected;
            default:
                return PrioritizationResponseStatus.Requested;
        }
    }
    private static SignalRequestMessage? MapToSignalRequestMessage(this SrmMessage request)
    {
        SrmMessageContent? content = request.SrmMessageContent.FirstOrDefault();
        if (content == null)
        {
            return null;
        }
        var srm = content.Srm;
        
        return new SignalRequestMessage
        {
            TimeStamp = srm.TimeStamp,
            Second = srm.Seconds,
            SequenceNumber = srm.SequenceNumber,
            Requests = srm.Requests.Select(r => new SignalRequestPackage
            {
                Request = new SignalRequest
                {
                    Id = r.Id.toIntersectionId(),
                    RequestId = r.RequestId,
                    RequestType = r.RequestType.ToSignalRequestType()
                }
            }).ToList(),
            Requestor = new RequestorDescription
            {
                Id = srm.Requestor.Id.ToVehicleId(),
                Name = srm.Requestor.Name,
                RouteName = srm.Requestor.RouteName,
                TransitSchedule = srm.Requestor.TransitSchedule,
                TransitOccupancy = srm.Requestor.TransitOccupancy.ToTransitVehicleOccupancy(),
                TransitStatus = new BitString(),
            }
        };
    }

    private static IntersectionReferenceId toIntersectionId(this RequestId id)
    {
        return new IntersectionReferenceId
        {
            Id = id.Id
        };
    }
    
    private static VehicleId ToVehicleId(this RequestorVehicleId requestorVehicleId)
    {
        if (requestorVehicleId.VehicleId != null)
        {
            return new VehicleId
            {
                EntityId = Encoding.UTF8.GetBytes(requestorVehicleId.VehicleId)
            };
        }

        return new VehicleId
        {
            StationId = requestorVehicleId.StationId
        };
    }

    private static PriorityRequestType ToSignalRequestType(this int id)
    {
        return (PriorityRequestType) id;
    }
    
    private static TransitVehicleOccupancy ToTransitVehicleOccupancy(this int occupancy)
    {
        return (TransitVehicleOccupancy) occupancy;
    }
}