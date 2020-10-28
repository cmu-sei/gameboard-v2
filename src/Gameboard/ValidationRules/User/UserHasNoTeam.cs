// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user is not a member of a team
    /// </summary>
    public class UserHasNoTeam :
        IValidationRule<TeamUserUpdate>,
        IValidationRule<TeamCreate>
    {
        public UserHasNoTeam(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
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

        public async Task Validate(TeamUserUpdate model)
        {
            if (DbContext.Users.Any(u => u.Id == IdentityId && u.TeamId != null))
                throw new InvalidModelException("User is already on a team.");
        }

        public async Task Validate(TeamCreate model)
        {
            if (DbContext.Users.Any(u => u.Id == IdentityId && u.TeamId != null))
                throw new InvalidModelException("User is already on a team.");
        }
    }
}

