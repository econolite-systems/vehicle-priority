// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Linq;
using System.Text.Json;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.Api;
using Xunit;

namespace Econolite.Ode.Model.VehiclePriority.Test;

public class BsmMessageTests
{
    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    
    [Fact]
    public void BsmMessage_String_ToJson()
    {
        var json = @"{
            ""BsmMessageContent"": [{
                ""metadata"": {
                    ""utctimestamp"": ""2022-07-18T17:04:23.347198Z""
                },
                ""bsm"": {
                    ""coreData"": {
                        ""msgCnt"": 0,
                        ""id"": 365779719,
                        ""secMark"": 365779719,
                        ""lat"": 425158240,
                        ""long"": -830466880,
                        ""elev"": 0,
                        ""accuracy"": {
                            ""semiMajor"": 0,
                            ""semiMinor"": 0,
                            ""orientation"": 0
                        },
                        ""transmission"": 7,
                        ""speed"": 558,
                        ""heading"": 28635,
                        ""angle"": 127,
                        ""accelSet"": {
                            ""long"": 0,
                            ""lat"": 0,
                            ""vert"": 0,
                            ""yaw"": 0
                        },
                        ""brakes"": {
                            ""wheelBrakes"": 0,
                            ""traction"": 0,
                            ""abs"": 0,
                            ""scs"": 0,
                            ""brakeBoost"": 0,
                            ""auxBrakes"": 0
                        },
                        ""size"": {
                            ""width"": 0,
                            ""length"": 0
                        }
                    }
                }
            }
            ]
        }";

        var bsm = JsonSerializer.Deserialize<BsmMessage>(json, _jsonOptions);
    }
    
    [Fact]
    public void BsmMessage_String_ToJson_ToVehicleUpdate()
    {
        var json = @"{
            ""BsmMessageContent"": [{
                ""metadata"": {
                    ""utctimestamp"": ""2022-07-18T17:04:23.347198Z""
                },
                ""bsm"": {
                    ""coreData"": {
                        ""msgCnt"": 0,
                        ""id"": 365779719,
                        ""secMark"": 365779719,
                        ""lat"": 425158240,
                        ""long"": -830466880,
                        ""elev"": 0,
                        ""accuracy"": {
                            ""semiMajor"": 0,
                            ""semiMinor"": 0,
                            ""orientation"": 0
                        },
                        ""transmission"": 7,
                        ""speed"": 558,
                        ""heading"": 28635,
                        ""angle"": 127,
                        ""accelSet"": {
                            ""long"": 0,
                            ""lat"": 0,
                            ""vert"": 0,
                            ""yaw"": 0
                        },
                        ""brakes"": {
                            ""wheelBrakes"": 0,
                            ""traction"": 0,
                            ""abs"": 0,
                            ""scs"": 0,
                            ""brakeBoost"": 0,
                            ""auxBrakes"": 0
                        },
                        ""size"": {
                            ""width"": 0,
                            ""length"": 0
                        }
                    }
                }
            }
            ]
        }";

        var bsm = JsonSerializer.Deserialize<BsmMessage>(json, _jsonOptions);
        if (bsm != null)
        {
            var vehicleUpdate = bsm.BsmMessageContent.First().ToVehicleUpdate();
        }
    }
}
