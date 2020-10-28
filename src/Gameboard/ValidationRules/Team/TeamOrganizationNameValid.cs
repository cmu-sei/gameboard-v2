// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team organization name is valid
    /// </summary>
    public class TeamOrganizationNameValid : 
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamCreate>,
        IValidationRule<UserEdit>,
        IValidationRule<TeamLock>,
        IValidationRule<TeamInviteCode>
    {
        OrganizationOptions OrganizationOptions { get; }
        GameboardDbContext DbContext { get; }

        public TeamOrganizationNameValid(OrganizationOptions organizationOptions, GameboardDbContext db)
        {
            OrganizationOptions = organizationOptions;
            DbContext = db;
        }

        public async Task Validate(TeamUpdate model)
        {
            Validate(model.OrganizationName, model.OrganizationLogoUrl);
        }

        public async Task Validate(TeamCreate model)
        {
            Validate(model.OrganizationName, model.OrganizationLogoUrl);
        }

        public async Task Validate(UserEdit model)
        {
            Validate(model.Organization, model.Organization);
        }

        void Validate(string name, string logoUrl)
        {
            if (OrganizationOptions.IsEnabled)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidModelException("Organization name is required.");

                if (!OrganizationOptions.Items.Any(i => i.Name.ToLower().Trim() == name.ToLower().Trim()))
                    throw new InvalidModelException("Invalid organization");

                //TODO: Move below to new OrganizationLogoIsValid rule

                string file = logoUrl.Split('/').LastOrDefault();
                if (!string.IsNullOrEmpty(file) && !file.StartsWith(name))
                    throw new InvalidModelException("Invalid organization logo");
            }
        }

        public async Task Validate(TeamLock model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId);
            Validate(team.OrganizationName, team.OrganizationLogoUrl);
        }

        public async Task Validate(TeamInviteCode model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId);
            Validate(team.OrganizationName, team.OrganizationLogoUrl);
        }
    }
}

