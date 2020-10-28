// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(UserCanEditTeam),
        typeof(TeamIsUnlocked),
        typeof(EnrollmentPeriodHasNotEnded),
        typeof(MinTeamSizeIsValid),
        typeof(MaxTeamSizeIsValid),
        typeof(TeamOrganizationNameValid)
    )]
    public class TeamLock
    {
        public string TeamId { get; set; }
    }
}

