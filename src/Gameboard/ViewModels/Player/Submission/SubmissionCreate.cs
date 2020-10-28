// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(ProblemIsValid),
        typeof(ChallengeIsNotDisabled),
        typeof(ProblemIsOwned),
        typeof(ProblemIsActive),
        typeof(SubmissionHasTokens),
        typeof(SubmissionDidNotExceedMaxSubmissions),
        typeof(BoardHasStarted),
        typeof(BoardHasNotEnded),
        typeof(TeamIsLocked),
        typeof(TeamDidNotExceedTimeLimit)
    )]
    public class SubmissionCreate
    {
        public string ProblemId { get; set; }

        public string[] Tokens { get; set; }
    }
}

