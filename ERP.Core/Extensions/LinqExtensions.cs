//-----------------------------------------------------------------------
// <copyright file="LinqExtensions.cs" company="sensyrtech">
//     transfercopy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Extensions
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Distinct the by.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>the list of items</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified predicate is any.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        ///   <c>true</c> if the specified predicate is any; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAny<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return source != null && source.Any(predicate);
        }

        /// <summary>
        /// Determines whether this instance is any.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   <c>true</c> if the specified source is any; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAny<TSource>(this IEnumerable<TSource> source)
        {
            return source != null && source.Any();
        }
    }
}
