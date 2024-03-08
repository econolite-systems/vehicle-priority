// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Text.Json;
using Econolite.Ode.Models.VehiclePriority.J2735;
using FluentAssertions;
using Xunit;

namespace Model.VehiclePriority.Test.J2735;

public class BsmCoreDataTests
{
    JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };
    
    [Fact]
    public void Create_BsmCoreData_FromRequiredValues()
    {
        var json = "{ \"msgCnt\": 88, \"id\": \"A0F1\", \"secMark\": 4567, \"lat\": 40741895, \"long\": -73989308, \"elev\": 3456 }";

        var actual = JsonSerializer.Deserialize<BsmCoreData>(json, JsonOptions);
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public void Create_BsmCoreData_FromFlagValues()
    {
        var json = "{ \"msgCnt\": 88, \"id\": \"A0F1\", \"secMark\": 4567, \"lat\": 40741895, \"long\": -73989308, \"elev\": 3456, \"transmission\": 7 }";

        var actual = JsonSerializer.Deserialize<BsmCoreData>(json, JsonOptions);
        actual.Should().NotBeNull();
    }
    
    
}
