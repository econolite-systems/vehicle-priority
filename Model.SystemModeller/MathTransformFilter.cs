// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems.Transformations;

namespace Econolite.Ode.Domain.SystemModeller
{
  public sealed class MathTransformFilter : ICoordinateSequenceFilter
  {
    private readonly MathTransform _transform;

    public MathTransformFilter(MathTransform transform) => this._transform = transform;

    public bool Done => false;

    public bool GeometryChanged => true;

    public void Filter(CoordinateSequence seq, int i)
    {
      double[] numArray = this._transform.Transform(new double[2]
      {
        seq.GetOrdinate(i, (Ordinate) 0),
        seq.GetOrdinate(i, (Ordinate) 1)
      });
      seq.SetOrdinate(i, (Ordinate) 0, numArray[0]);
      seq.SetOrdinate(i, (Ordinate) 1, numArray[1]);
    }
  }
}
