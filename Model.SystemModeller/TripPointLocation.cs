// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using NetTopologySuite.Geometries;

namespace Econolite.Ode.Model.SystemModeller;

public record TripPointLocation(int Distance, double[] Point);
