// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard
{
    /// <summary>
    /// ienumerable extensions
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// checks if source collection contains target collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> target)
            where T : class
        {
            foreach(var value in target)
            {
                if (!source.Contains(value)) return false;
            }

            return true;
        }
    }
}

