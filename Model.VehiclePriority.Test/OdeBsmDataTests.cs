// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Econolite.Ode.Models.VehiclePriority;
using Econolite.Ode.Models.VehiclePriority.J2735;
using Xunit;

namespace Econolite.Ode.Model.VehiclePriority.Test;

public class OdeBsmDataTests
{
    JsonSerializerOptions JsonOptions = new JsonSerializerOptions
     {
         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
         PropertyNameCaseInsensitive = true,
         WriteIndented = true,
         Converters ={
           new JsonStringEnumConverter()
         },
     };
    
    [Fact]
    public void Test1()
    {
        var json = @"{
          ""metadata"": {
            ""bsmSource"": ""EV"",
            ""logFileName"": ""bsmTx.gz"",
            ""recordType"": ""bsmTx"",
            ""securityResultCode"": ""success"",
            ""receivedMessageDetails"": {
              ""locationData"": {
                ""latitude"": ""40.5657881"",
                ""longitude"": ""-105.0316742"",
                ""elevation"": ""1489"",
                ""speed"": ""0.4"",
                ""heading"": ""267.4""
              },
              ""rxSource"": ""NA""
            },
            ""payloadType"": ""us.dot.its.jpo.ode.model.OdeBsmPayload"",
            ""serialId"": {
              ""streamId"": ""f0eddcb8-9051-4f28-ad8f-3a9d35eba1b5"",
              ""bundleSize"": 16,
              ""bundleId"": 0,
              ""recordId"": 15,
              ""serialNumber"": 15
            },
            ""odeReceivedAt"": ""2022-07-07T16:46:50.943Z"",
            ""schemaVersion"": 6,
            ""maxDurationTime"": 0,
            ""odePacketID"": """",
            ""odeTimStartDateTime"": """",
            ""recordGeneratedAt"": ""2018-05-01T15:55:55.494Z"",
            ""recordGeneratedBy"": ""OBU"",
            ""sanitized"": false
          },
          ""payload"": {
            ""dataType"": ""us.dot.its.jpo.ode.plugin.j2735.J2735Bsm"",
            ""data"": {
              ""coreData"": {
                ""msgCnt"": 119,
                ""id"": ""31325433"",
                ""secMark"": 55494,
                ""position"": {
                  ""latitude"": 40.5657881,
                  ""longitude"": -105.0316742,
                  ""elevation"": 1489.0
                },
                ""accelSet"": {
                  ""accelLat"": 0.00,
                  ""accelLong"": 0.43,
                  ""accelVert"": 0.00,
                  ""accelYaw"": 0.00
                },
                ""accuracy"": {
                  ""semiMajor"": 9.30,
                  ""semiMinor"": 12.05
                },
                ""transmission"": ""NEUTRAL"",
                ""speed"": 0.40,
                ""heading"": 267.4000,
                ""brakes"": {
                  ""wheelBrakes"": {
                    ""leftFront"": false,
                    ""rightFront"": false,
                    ""unavailable"": true,
                    ""leftRear"": false,
                    ""rightRear"": false
                  },
                  ""traction"": ""unavailable"",
                  ""abs"": ""unavailable"",
                  ""scs"": ""unavailable"",
                  ""brakeBoost"": ""unavailable"",
                  ""auxBrakes"": ""unavailable""
                },
                ""size"": {
                  ""width"": 190,
                  ""length"": 570
                }
              },
              ""partII"": [
                {
                  ""id"": ""VehicleSafetyExtensions"",
                  ""value"": {
                    ""pathHistory"": {
                      ""crumbData"": [
                        {
                          ""elevationOffset"": 1.0,
                          ""latOffset"": 0.0000051,
                          ""lonOffset"": 0.0000097,
                          ""timeOffset"": 0.90
                        },
                        {
                          ""elevationOffset"": 4.7,
                          ""latOffset"": 0.0000141,
                          ""lonOffset"": 3E-7,
                          ""timeOffset"": 3.69
                        },
                        {
                          ""elevationOffset"": 9.4,
                          ""latOffset"": 0.0000341,
                          ""lonOffset"": 0.0000326,
                          ""timeOffset"": 8.29
                        },
                        {
                          ""elevationOffset"": 13.5,
                          ""latOffset"": 0.0000438,
                          ""lonOffset"": 0.0000839,
                          ""timeOffset"": 18.29
                        },
                        {
                          ""elevationOffset"": 12.0,
                          ""latOffset"": 0.0000484,
                          ""lonOffset"": 0.0001072,
                          ""timeOffset"": 26.49
                        },
                        {
                          ""elevationOffset"": 11.7,
                          ""latOffset"": 0.0000478,
                          ""lonOffset"": 0.0001117,
                          ""timeOffset"": 28.00
                        },
                        {
                          ""elevationOffset"": 11.3,
                          ""latOffset"": 0.0000320,
                          ""lonOffset"": 0.0001211,
                          ""timeOffset"": 28.80
                        },
                        {
                          ""elevationOffset"": 11.2,
                          ""latOffset"": 0.0000301,
                          ""lonOffset"": 0.0001197,
                          ""timeOffset"": 30.00
                        },
                        {
                          ""elevationOffset"": 7.8,
                          ""latOffset"": 0.0000202,
                          ""lonOffset"": 0.0001092,
                          ""timeOffset"": 34.90
                        },
                        {
                          ""elevationOffset"": 1.9,
                          ""latOffset"": 0.0000230,
                          ""lonOffset"": 0.0001186,
                          ""timeOffset"": 40.00
                        },
                        {
                          ""elevationOffset"": -0.1,
                          ""latOffset"": 0.0000248,
                          ""lonOffset"": 0.0001401,
                          ""timeOffset"": 42.8
                        },
                        {
                          ""elevationOffset"": -3.3,
                          ""latOffset"": 0.0000343,
                          ""lonOffset"": 0.0001451,
                          ""timeOffset"": 49.80
                        },
                        {
                          ""elevationOffset"": -3.6,
                          ""latOffset"": 0.0000443,
                          ""lonOffset"": 0.0001437,
                          ""timeOffset"": 51.60
                        },
                        {
                          ""elevationOffset"": -6.9,
                          ""latOffset"": 0.0000466,
                          ""lonOffset"": 0.0001419,
                          ""timeOffset"": 56.49
                        },
                        {
                          ""elevationOffset"": -8.4,
                          ""latOffset"": 0.0000346,
                          ""lonOffset"": 0.0001261,
                          ""timeOffset"": 62.49
                        }
                      ]
                    },
                    ""pathPrediction"": {
                      ""confidence"": 0.0,
                      ""radiusOfCurve"": 0.0
                    }
                  }
                },
                {
                  ""id"": ""SupplementalVehicleExtensions"",
                  ""value"": {}
                }
              ]
            }
          }
        }";
        var actual = JsonSerializer.Deserialize<OdeBsmData>(json, JsonOptions);
    }

    [Fact]
    public void HexString_ToByteArray()
    {
        // var hex = "001480CF4B950C400022D2666E923D1EA6D4E28957BD55FFFFF001C758FD7E67D07F7FFF8000000002020218E1C1004A40196FBC042210115C030EF1408801021D4074CE7E1848101C5C0806E8E1A50101A84056EE8A1AB4102B840A9ADA21B9010259C08DEE1C1C560FFDDBFC070C0222210018BFCE309623120FFE9BFBB10C8238A0FFDC3F987114241610009BFB7113024780FFAC3F95F13A26800FED93FDD51202C5E0FE17BF9B31202FBAFFFEC87FC011650090019C70808440C83207873800000000001095084081C903447E31C12FC0";
        //var hex = "25000a8000000000267562cf1672de3d88000000000070f60e7afd7d07d07f7fff0000000000";//"001425000a8000000000267562cf1672de3d88000000000070f60e7afd7d07d07f7fff0000000000";
        var hex = "006f754d19a4a15cce6706000004000927a92ce25a8f01000600038100400380";
        //var result = hex.ToBytes();
        var result = BsmExtensions.StringToByteArrayFastest(hex);
        var bsm = StructureSerialize.ReadStruct<RawBsm>(result);
        // var json = System.Text.Encoding.Default.GetString(result);
    }

    [Fact]
    public void HexStringBsm_ToByteArray()
    {
      var hex = "25000a8000000000267562cf1672de3d88000000000070f60e7afd7d07d07f7fff0000000000";
      var result = BsmExtensions.StringToByteArrayFastest(hex);
      var bsm = StructureSerialize.ReadStruct<RawBsm>(result);
      var payload = result.Skip(result.Length - bsm.PayloadLength).ToArray();
      // var json = System.Text.Encoding.Default.GetString(result);
    }
    
    [Fact]
    public void HexStringLocation_ToByteArray()
    {
      var hex = "006f754d19a4a15cce6706000004000927a92ce25a8f01000600038100400380";
      var result = BsmExtensions.StringToByteArrayFastest(hex);
      var bsm = StructureSerialize.ReadStruct<RawBsm>(result);
      var payload = result.Skip(result.Length - bsm.PayloadLength).ToArray();
      
      // var json = System.Text.Encoding.Default.GetString(result);
    }
    
    [Fact]
    public void ConvertByteArrayToInt()
    {
        var hex = "6f754d19";
        var bytes = BsmExtensions.StringToByteArrayFastest(hex);
        //Array.Reverse(bytes);

        int i = BitConverter.ToInt32(bytes, 0);
    }
}

public static class BsmExtensions
{
    public static byte[] StringToByteArrayFastest(string hex) {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static int GetHexVal(char hex) {
        int val = (int)hex;
        //For uppercase A-F letters:
        //return val - (val < 58 ? 48 : 55);
        //For lowercase a-f letters:
        //return val - (val < 58 ? 48 : 87);
        //Or the two combined, but a bit slower:
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }
  
}

public static class HexUtil
{
  private readonly static Dictionary<char, byte> hexmap = new Dictionary<char, byte>()
  {
    { 'a', 0xA },{ 'b', 0xB },{ 'c', 0xC },{ 'd', 0xD },
    { 'e', 0xE },{ 'f', 0xF },{ 'A', 0xA },{ 'B', 0xB },
    { 'C', 0xC },{ 'D', 0xD },{ 'E', 0xE },{ 'F', 0xF },
    { '0', 0x0 },{ '1', 0x1 },{ '2', 0x2 },{ '3', 0x3 },
    { '4', 0x4 },{ '5', 0x5 },{ '6', 0x6 },{ '7', 0x7 },
    { '8', 0x8 },{ '9', 0x9 }
  };
  public static byte[] ToBytes(this string hex)
  {
    if (string.IsNullOrWhiteSpace(hex))
      throw new ArgumentException("Hex cannot be null/empty/whitespace");

    if (hex.Length % 2 != 0)
      throw new FormatException("Hex must have an even number of characters");

    bool startsWithHexStart = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase);

    if (startsWithHexStart && hex.Length == 2)
      throw new ArgumentException("There are no characters in the hex string");


    int startIndex = startsWithHexStart ? 2 : 0;

    byte[] bytesArr = new byte[(hex.Length - startIndex) / 2];

    char left;
    char right;

    try 
    { 
      int x = 0;
      for(int i = startIndex; i < hex.Length; i += 2, x++)
      {
        left = hex[i];
        right = hex[i + 1];
        bytesArr[x] = (byte)((hexmap[left] << 4) | hexmap[right]);
      }
      return bytesArr;
    }
    catch(KeyNotFoundException)
    {
      throw new FormatException("Hex string has non-hex character");
    }
  }
}
