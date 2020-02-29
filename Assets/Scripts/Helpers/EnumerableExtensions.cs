﻿using System;
using System.Collections.Generic;

namespace R1Engine
{
    /// <summary>
    /// Extension method for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an item matching the predicate in an enumerable
        /// </summary>
        /// <typeparam name="T">The type of objects in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable</param>
        /// <param name="match">The predicate used to find the matching item</param>
        /// <returns>The item matching the predicate, or the default value if none was found</returns>
        /// <exception cref="ArgumentNullException"/>
        public static T FindItem<T>(this IEnumerable<T> enumerable, Predicate<T> match)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            foreach (T item in enumerable)
                if (match(item))
                    return item;

            return default;
        }

        /// <summary>
        /// Returns the index of an item matching the predicate in a list
        /// </summary>
        /// <typeparam name="T">The type of objects in the list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="match">The predicate used to find the matching item index</param>
        /// <returns>The item index matching the predicate, or -1 if none was found</returns>
        /// <exception cref="ArgumentNullException"/>
        public static int FindItemIndex<T>(this IList<T> list, Predicate<T> match)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            for (int i = 0; i < list.Count; i++)
                if (match(list[i]))
                    return i;

            return -1;
        }
    }
}
