// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class BoardCompletionReport
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public BoardType BoardType { get; set; }

        public List<BoardCompletionReportItem> Items { get; set; } = new List<BoardCompletionReportItem>();
    }

    public class BoardCompletionReportItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<BoardCompletionReportChallenge> Challenges { get; set; } = new List<BoardCompletionReportChallenge>();
    }

    public class BoardCompletionReportChallenge
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public BoardCompletionReportChallengeStat Success { get; set; } = new BoardCompletionReportChallengeStat();
        public BoardCompletionReportChallengeStat Failure { get; set; } = new BoardCompletionReportChallengeStat();
        public BoardCompletionReportChallengeStat Partial { get; set; } = new BoardCompletionReportChallengeStat();
        public int Total { get; set; }        
        public int Points { get; internal set; }
        public double AverageMilliseconds { get; set; }
    }

    public class BoardCompletionReportChallengeStat
    {
        public int Count { get; set; }
        public double Ratio { get; set; }
    }
}

