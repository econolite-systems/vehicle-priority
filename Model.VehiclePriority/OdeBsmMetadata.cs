// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using System.Collections.Generic;

namespace Econolite.Ode.Models.VehiclePriority;

public record OdeBsmMetadata(
    string LogFileName,
    RecordType RecordType,
    SecurityResultCode SecurityResultCode,
    ReceivedMessageDetails ReceivedMessageDetails,
    IEnumerable<As1Encoding> Encodings,
    BsmSource BsmSource,
    string OriginIp
);
