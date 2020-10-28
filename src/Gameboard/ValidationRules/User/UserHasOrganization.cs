// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user organization has been assigned
    /// </summary>
    public class UserHasOrganization :
        IValidationRule<TeamCreate>,
        IValidationRule<TeamUserUpdate>
    {
        public UserHasOrganization(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        string IdentityId { get { return Identity?.Id; } }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(TeamCreate model)
        {
            await Validate();
        }

        public async Task Validate(TeamUserUpdate model)
        {
            await Validate();
        }

        async Task Validate()
        {
            if (await DbContext.Users.AnyAsync(u => u.Id == IdentityId && string.IsNullOrWhiteSpace(u.Organization)))
                throw new InvalidModelException("User must select an organization.");
        }
    }
}

