using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> Generate<T>(this T seed, Func<T, T> step)
        {
            yield return seed;

            while (null != (seed = step(seed)))
            {
                yield return seed;
            }
        }
    }
}