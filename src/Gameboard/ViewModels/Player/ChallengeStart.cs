// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using GameEngine.Models;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{   

    [Validation(
        typeof(ChallengeIsValid),
        typeof(ChallengeIsNotDisabled),
        typeof(BoardHasStarted),
        typeof(BoardHasNotEnded),
        typeof(TeamIsLocked),
        typeof(TeamHasTeamBoard),
        typeof(TeamDidNotExceedTimeLimit),
        typeof(TeamDidNotExceedMaxConcurrentProblems)
    )]
    public class ChallengeStart
    {
        public string ChallengeId { get; set; }

        public int? FlagIndex { get; set; }
    }
}

