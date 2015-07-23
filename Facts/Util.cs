using System.Collections.Generic;

namespace Theraot.Facts
{
    internal static class Util
    {
        public static void Add<T>(this IDictionary<T, List<int>> dict, T key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Add(value);
            }
            else
            {
                dict[key] = new List<int> { value };
            }
        }

        public static void Remove<T>(this IDictionary<T, List<int>> dict, T key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key].Remove(value);
            }
        }
    }
}