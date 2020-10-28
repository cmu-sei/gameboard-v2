// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Gameboard
{
    /// <summary>
    /// string extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// converts a space-delimited string into a list of gameboard badges sorted alphabetically        ///
        /// </summary>
        /// <param name="badges"></param>
        /// <returns></returns>
        public static List<string> ToBadgeArray(this string badges)
        {
            if (string.IsNullOrWhiteSpace(badges))
                return new List<string>();

            return (badges ?? "").Split(' ').ToList();
        }
    }
}

