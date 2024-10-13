using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UTSAlgoEvolusi.Core.Utils;

public class CustomDoubleEqualityComparer : IEqualityComparer<double>
{
    private readonly double _threshold;

    public CustomDoubleEqualityComparer(double threshold)
    {
        _threshold = threshold;
    }

    public bool Equals(double x, double y)
    {
        var selisih = Math.Abs(x - y);

        return selisih <= _threshold;
    }

    public int GetHashCode([DisallowNull] double obj)
    {
        return obj.GetHashCode();
    }
}
