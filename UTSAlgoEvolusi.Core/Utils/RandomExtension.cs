using System;
using System.Collections.Generic;
using System.Linq;

namespace UTSAlgoEvolusi.Core.Utils;

public static class RandomExtension
{
    public static double NextDouble(this Random random, double minValue, double maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");

        var value = random.NextDouble();

        return value * (maxValue - minValue) + minValue;
    }

    public static int Next(this Random random, int minValue, int maxValue, int exclude)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");

        if (exclude < minValue || exclude >= maxValue)
            throw new ArgumentException("exlcude must between minValue and maxValue");

        var value = random.Next(minValue, maxValue);
        while (value == exclude)
            value = random.Next(minValue, maxValue);

        return value;
    }

    public static int Next(this Random random, int minValue, int maxValue, IEnumerable<int> excludes)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");

        if (excludes.Any(x => x < minValue || x >= maxValue))
            throw new ArgumentException("All element of exlcudes must between minValue and maxValue");

        if (excludes.Count() == maxValue - minValue)
            throw new ArgumentException("All number between minValue and maxValue are excluded");

        var value = random.Next(minValue, maxValue);
        while (excludes.Contains(value))
            value = random.Next(minValue, maxValue);

        return value;
    }

    public static T GetItem<T>(this Random random, IEnumerable<T> choices)
    {
        return random.GetItems(choices.ToArray(), 1)[0];
    }
}
