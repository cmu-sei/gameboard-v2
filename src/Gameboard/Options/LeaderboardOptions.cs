// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard
{
    /// <summary>
    /// leaderboard configuration options 
    /// </summary>
    public class LeaderboardOptions
    {
        public double IntervalMinutes { get; set; }

        public string CacheKey { get; set; }

        public bool Anonymize { get; set; }
    }
}

