// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard
{
    public class Leaderboard
    {
        public List<LeaderboardScore> Results { get; set; } = new List<LeaderboardScore>();

        public bool IsEmpty { get { return !Results.Any(); } }

        public string BoardId { get; set; }

        public int Total { get; set; }

        public IDataFilter<LeaderboardScore> DataFilter { get; set; }

        public DateTime Timestamp { get; set; }

        public int TotalActive { get; set; }

        public int TotalTeams { get; set; }
    }
}

