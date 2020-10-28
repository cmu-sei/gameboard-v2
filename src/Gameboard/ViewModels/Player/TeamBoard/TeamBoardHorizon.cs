// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class TeamBoardEventDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public DateTime Start { get; set; }

        public BoardDetail Board { get; set; }

        public int? OverrideMaxMinutes { get; set; }

        public double? Score { get; set; }

        public List<ChallengeEventDetail> Challenges { get; set; } = new List<ChallengeEventDetail>();        
    }

    public class ChallengeEventDetail
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public int Points { get; set; }
        public string Name { get; set; }
        public List<ProblemEventDetail> Events { get; set; } = new List<ProblemEventDetail>();
        public string Type { get; set; }
    }

    public class ProblemEventDetail
    {
        public ProblemEventType Type { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum ProblemEventType
    { 
        Default = 0,
        Failure = 1,
        Partial = 2,
        Success = 3
    }
}

