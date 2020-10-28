// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(TeamNameIsValid),
        typeof(UserCanEditTeam),
        typeof(TeamIsUnlocked),
        typeof(TeamNameIsAvailable),
        typeof(TeamOrganizationNameValid),
        typeof(EnrollmentPeriodHasNotEnded)
    )]
    public class TeamUpdate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OrganizationLogoUrl { get; set; }
        public string OrganizationalUnitLogoUrl { get; set; }
        public string OrganizationName { get; set; }
    }
}

