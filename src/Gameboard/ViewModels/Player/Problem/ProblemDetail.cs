// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;

namespace Gameboard.ViewModels
{
    public class ProblemDetail
    {
        public string Id { get; set; }

        public string ChallengeLinkId { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        /// <summary>
        /// set by <see cref="GameEngine.Abstractions.IGameEngineEventHandler"/>
        /// </summary>
        public int EstimatedReadySeconds { get; set; }

        public long Score { get; set; }

        public string Status { get; set; }

        public string Text { get; set; }

        public bool HasGamespace { get; set; }

        public bool GamespaceReady { get; set; }

        public string GamespaceText { get; set; }

        public SubmissionDetail[] Submissions { get; set; }

        public TokenDetail[] Tokens { get; set; }

        public string TeamId { get; set; }

        public string Slug { get; set; }

        public string SharedId { get; set; }
    }
}

