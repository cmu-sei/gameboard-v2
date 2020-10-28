// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System;

namespace Gameboard
{
    /// <summary>
    /// team board extension
    /// </summary>
    public static class TeamBoardExtensions
    {
        /// <summary>
        /// determine if the team board is active based on board max minutes or team override max minutes
        /// </summary>
        /// <param name="teamBoard"></param>
        /// <returns></returns>
        public static bool IsActive(this TeamBoard teamBoard, BoardDetail board)
        {
            if (board.IsActive())
            {
                int duration = teamBoard.OverrideMaxMinutes ?? board?.MaxMinutes ?? 0;
                return duration > 0
                    ? teamBoard.Start.AddMinutes(duration).CompareTo(DateTime.UtcNow) > 0
                    : false;
            }

            return false;
        }
    }
}

