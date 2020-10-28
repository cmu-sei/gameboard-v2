// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(TeamIsUnlocked),
        typeof(UserCanEditTeam),
        typeof(EnrollmentPeriodHasNotEnded),
        typeof(TeamOrganizationNameValid)
    )]
    public class TeamInviteCode
    {
        public string TeamId { get; set; }

        public string InvitationCode { get; set; }
    }
}

