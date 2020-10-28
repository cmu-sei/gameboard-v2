// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(TeamInviteCodeIsValid),
        // typeof(UserHasNoTeam),
        typeof(TeamIsUnlocked),
        typeof(MaxTeamSizeIsValid),
        typeof(EnrollmentPeriodHasNotEnded),
        typeof(UserHasOrganization)
    )]
    public class TeamUserUpdate
    {
        public string TeamId { get; set; }

        public string InviteCode { get; set; }
    }
}

