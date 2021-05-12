using System;
using System.Collections.Generic;
using System.Linq;

namespace TGPToolchain.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Ensures that elements only exist a single time in a list and returning the last instance of them, based on a selector.
        /// </summary>
        /// <param name="entities">List of LiteDB entities</param>
        /// <param name="keySelector">Selector for the internal grouping function. Should return a grouping key.</param>
        /// <typeparam name="T">Type of the elements</typeparam>
        /// <typeparam name="TKey">Type of the grouping key</typeparam>
        /// <returns>A list where every element only exists once, based on the grouping key.</returns>
        public static IEnumerable<T> DistinctLast<T, TKey>(this IEnumerable<T> elements, Func<T, TKey> keySelector)
        {
            return elements.GroupBy(keySelector).Select(grouped => grouped.Last());
        }
    }
}