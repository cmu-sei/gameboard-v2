// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(UserCanEditTeamBoard))]
    public class TeamBoardUpdate
    {
        public string BoardId { get; set; }

        public string TeamId { get; set; }

        public int? OverrideMaxMinutes { get; set; }
    }
}
