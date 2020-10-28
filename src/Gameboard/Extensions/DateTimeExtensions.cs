// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System;

namespace Gameboard
{
    /// <summary>
    /// date time extensions
    /// </summary>
    public static class DateTimeExtensions
    {        
        /// <summary>
        /// check if stop time has value and has not ended
        /// </summary>
        /// <param name="stopTime"></param>
        /// <returns></returns>
        internal static bool HasNotEnded(DateTime? stopTime)
        {
            return (stopTime.HasValue && DateTime.UtcNow.CompareTo(stopTime) < 0)
                || !stopTime.HasValue;
        }

        /// <summary>
        /// check if start time has value and has started
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        internal static bool HasStarted(DateTime? startTime)
        {
            return (startTime.HasValue && DateTime.UtcNow.CompareTo(startTime) > 0)
                || !startTime.HasValue;
        }
    }
}

