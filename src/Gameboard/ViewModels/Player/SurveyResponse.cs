// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class SurveyResponse
    {
        public string Text { get; set; }

        public string Value { get; set; }
    }

    public class ChallengeSurveyReport
    {
        public string BoardId { get; set; }

        public string BoardName { get; set; }

        public List<ChallengSurveyItem> Items { get; set; } = new List<ChallengSurveyItem>();
    }

    public class ChallengSurveyItem
    {
        public string ChallengeId { get; set; }
        public string ChallengeTitle { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }        
        public DateTime Date { get; set; }
    }

    public class SurveyReport
    {
        public string GameId { get; set; }

        public string GameName { get; set; }

        public List<SurveyItem> Items { get; set; } = new List<SurveyItem>();
    }

    public class SurveyItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }        
        public DateTime Date { get; set; }
    }
}

