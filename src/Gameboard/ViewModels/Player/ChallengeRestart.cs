// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(ProblemIsValid),
        typeof(ProblemHasIssues),
        typeof(UserCanEditTeam),
        typeof(BoardHasNotEnded)
    )]
    public class ChallengeRestart
    {
        public string ChallengeLinkId { get; set; }

        public string TeamId { get; set; }
    }
}

