// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;

namespace Econolite.Client.VehiclePriority.Model.J2735;


public class SrmMessage
{
  public SrmMessageContent[] SrmMessageContent { get; set; }
}

public class SrmMessageContent
{
  public SrmMessageMetadata Metadata { get; set; }
  public SrmMessagePayload Srm { get; set; }
}

public class SrmMessageMetadata
{
  public DateTime Utctimestamp { get; set; }
}


public class SrmMessagePayload
{
  public int SequenceNumber { get; set; }
  public int TimeStamp { get; set; }
  public int Seconds { get; set; }
  public Requestor Requestor { get; set; }
  public IEnumerable<Request> Requests { get; set; }
}

public class Requestor
{
  public RequestorVehicleId Id { get; set; }
  public string Name { get; set; }
  public string RouteName { get; set; }
  public int TransitSchedule { get; set; }
  public int TransitOccupancy { get; set; }
  public RequestorTransitStatus TransitStatus { get; set; }
  public RequestorType Type { get; set; }
  public RequestorPosition Position { get; set; }
}

public class RequestorVehicleId
{
  public string? VehicleId { get; set; }
  public int? StationId { get; set; }
}

public class RequestorTransitStatus
{
  public int BitsUsed { get; set; }
  public string Bits { get; set; }
}

public class RequestorType
{
  public int Role { get; set; }
}

public class RequestorPosition
{
  public long Heading { get; set; }
  public Position Position { get; set; }
  public int Speed { get; set; }
}

public class Position
{
  public int Latitude { get; set; }
  public int Longitude { get; set; }
}

public class SrmRequest
{
  public Request Request { get; set; }
  public int Minute { get; set; }
  public int Duration { get; set; }
  public int Second { get; set; }
}

public class Request
{
  public RequestId Id { get; set; }
  public int RequestId { get; set; }
  public int RequestType { get; set; }
  public RequestLane InBoundLane { get; set; }
  public RequestLane OutBoundLane { get; set; }
}

public class RequestId
{
  public int? Region { get; set; }
  public int Id { get; set; }
}

public class RequestLane
{
  public int Lane { get; set; }
  public int Approach { get; set; }
  public int Connection { get; set; }
}
