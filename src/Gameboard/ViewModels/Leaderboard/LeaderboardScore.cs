// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using System;
using System.Collections.Generic;

namespace Gameboard
{
    public class LeaderboardScore : ITeamModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> Badges { get; set; }
        public bool IsDisabled { get; set; }

        public string AnonymizedName { get; set; }

        public int Number { get; set; }

        public double Score { get; set; }

        public LeaderboardScoreCounts Counts { get; set; } = new LeaderboardScoreCounts();

        public double Duration { get; set; }

        public int Rank { get; set; }

        public string OrganizationLogoUrl { get; set; }

        public string OrganizationalUnitLogoUrl { get; set; }

        public string OrganizationName { get; set; }

        public DateTime? Start { get; set; }

        public int MaxMinutes { get; set; }
    }

    public class LeaderboardScoreCounts
    {
        public int Partial { get; set; }

        public int Failure { get; set; }

        public int Success { get; set; }

        public int Total { get; set; }
    }

    public class LeaderboardExport
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Badges { get; set; }
        public bool IsDisabled { get; set; }

        public string AnonymizedName { get; set; }

        public int Number { get; set; }

        public double Score { get; set; }

        public int PartialCount { get; set; }

        public int FailureCount { get; set; }

        public int SuccessCount { get; set; }

        public int TotalCount { get; set; }

        public double Duration { get; set; }

        public int Rank { get; set; }

        public string OrganizationLogoUrl { get; set; }

        public string OrganizationalUnitLogoUrl { get; set; }

        public string OrganizationName { get; set; }

        public DateTime? Start { get; set; }

        public int MaxMinutes { get; set; }
    }
}

