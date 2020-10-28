// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(TeamIsLocked),
        typeof(BoardHasStarted),
        typeof(BoardHasNotEnded),
        typeof(UserCanEditTeam),
        typeof(MinTeamSizeIsValid),
        typeof(MaxTeamSizeIsValid),
        typeof(TeamHasBoardPrerequisites),
        typeof(TeamHasNoSession)
    )]
    public class BoardStart
    {
        public string Id { get; set; }
    }
}

