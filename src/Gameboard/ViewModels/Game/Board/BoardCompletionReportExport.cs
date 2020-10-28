// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;

namespace Gameboard.ViewModels
{
    public class BoardCompletionReportExport
    {
        public string BoardId { get; set; }
        public string BoardName { get; set; }
        public BoardType BoardType { get; set; }
        public string ContainerId { get; set; }
        public string ContainerName { get; set; }
        public string ChallengeId { get; set; }
        public string ChallengeTitle { get; set; }
        public int SuccessCount { get; set; }
        public double SuccessRatio { get; set; }
        public int FailureCount { get; set; }
        public double FailureRatio { get; set; }
        public int PartialCount { get; set; }
        public double PartialRatio { get; set; }
        public int Total { get; set; }
        public int Points { get; internal set; }
        public double AverageMilliseconds { get; set; }
    }
}

