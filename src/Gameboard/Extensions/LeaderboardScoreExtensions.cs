// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;

namespace Gameboard
{
    /// <summary>
    /// extensions for leaderboard score
    /// </summary>
    public static class LeaderboardScoreExtensions
    {
        /// <summary>
        /// checks if a leaderboard score is still active
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public static bool IsActive(this LeaderboardScore score)
        {
            if (score.Start.HasValue)
                return score.Start.Value.AddMinutes(score.MaxMinutes) > DateTime.UtcNow;

            return false;
        }
    }
}

