// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Ode.Models.VehiclePriority;
using FluentAssertions;
using MongoDB.Bson.Serialization.Conventions;
using Xunit;

namespace Econolite.Ode.Model.VehiclePriority.Test;

public class PriortyRequestMessageTest
{
    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters ={
            new JsonStringEnumConverter()
        },
    };
    
    [Fact]
    public void ConvertFromRouteStatus_ToRequest_InitialRequest()
    {
        var status = CreateRouteStatus(PriorityRequestType.Initial);
        var result = status.ToPriorityRequestMessage();

        var expected = new PriorityRequestMessage(PriorityRequestType.Initial, new PRequest(
            1, "Test", 10, 1, 2, 240, 245));
        result.Should().NotBeNull();
        result.Should().Be(expected);
    }
    
    [Fact]
    public void ConvertFromRouteStatus_ToRequest_UpdateRequest()
    {
        var status = CreateRouteStatus(PriorityRequestType.Update);
        var result = status.ToPriorityRequestMessage();

        var expected = new PriorityRequestMessage(PriorityRequestType.Update, new PRequest(
            1, "Test", 10, 1, 2, 240, 245));
        result.Should().NotBeNull();
        result.Should().Be(expected);
    }
    
    [Fact]
    public void ConvertFromRouteStatus_ToRequest_CancelRequest()
    {
        var status = CreateRouteStatus(PriorityRequestType.Cancel);
        var result = status.ToPriorityRequestMessage();

        var expected = new PriorityRequestMessage(PriorityRequestType.Cancel, new PRequest(
            1, "Test", 10, 1, 2, null, null));
        result.Should().NotBeNull();
        result.Should().Be(expected);
    }
    
    [Fact]
    public void ConvertFromRouteStatus_ToRequestJson_CancelRequest()
    {
        var status = CreateRouteStatus(PriorityRequestType.Cancel);
        var result = status.ToPriorityRequestMessage();

        var expected = new PriorityRequestMessage(PriorityRequestType.Cancel, new PRequest(
            1, "Test", 10, 1, 2, null, null));
        result.Should().NotBeNull();
        result.Should().Be(expected);

        var json = JsonSerializer.Serialize(result, _jsonOptions);
    }

    private RouteStatus CreateRouteStatus(PriorityRequestType type)
    {
        var status = new RouteStatus()
        {
            Id = Guid.Empty,
            RouteId = Guid.Empty,
            RequestId = 1,
            DesiredClassLevel = 1,
            VehicleId = "Test",
            NextIntersection = new Controller()
            {
                IntersectionId = Guid.Empty,
                Plan = 2
            },
            EtaInSeconds = 240,
            IsInitial = type == PriorityRequestType.Initial,
            Completed = type == PriorityRequestType.Cancel
        };

        return status;
    }
}
