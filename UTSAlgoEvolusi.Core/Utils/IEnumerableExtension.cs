using System.Collections.Generic;
using System.Linq;

namespace UTSAlgoEvolusi.Core.Utils
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> enumerable, int length, T value)
        {
            if (enumerable.Count() < length)
            {
                var result = new List<T>();
                var selisih = length - enumerable.Count();

                for(int i = 0; i < selisih; i++)
                {
                    result.Add(value);
                }

                result.AddRange(enumerable);
                return result;
            }

            return enumerable;
        }
    }
}
