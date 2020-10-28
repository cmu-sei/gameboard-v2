// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    public class GamespaceDetail
    {
        public string Id { get; set; }
        public DateTime Start { get; set; }
        public string ChallengeId { get; set; }
        public string ChallengeSlug { get; set; }
        public string ChallengeTitle { get; set; }
        public int ChallengePoints { get; set; }
        public string ChallengeCategoryName { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamOrganizationName { get; set; }
        public string TeamNumber { get; set; }
    }

    [Validation(
        typeof(ProblemIsValid),
        typeof(ProblemIsOwned),
        typeof(ProblemIsActive),
        typeof(TeamDidNotExceedTimeLimit),
        typeof(TeamDidNotExceedMaxConcurrentProblems)
    )]
    public class GamespaceRestart
    {
        public string ProblemId { get; set; }
    }

    [Validation(
        typeof(ProblemIsOwned)
    )]
    public class GamespaceDelete
    {
        public string Id { get; set; }

    }
}

