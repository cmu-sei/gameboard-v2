// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(UserCanEditTeam),
        typeof(UserIsTeamMember),
        typeof(TeamIsUnlocked),
        typeof(EnrollmentPeriodHasNotEnded)
    )]
    public class TeamUserDelete
    {
        public string TeamId { get; set; }

        public string UserId { get; set; }
    }
}

