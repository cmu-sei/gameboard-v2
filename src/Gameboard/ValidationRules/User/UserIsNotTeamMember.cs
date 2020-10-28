// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user is not already on team
    /// </summary>
    public class UserIsNotTeamMember : IValidationRule<TeamUserUpdate>
    {
        public UserIsNotTeamMember(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(TeamUserUpdate model)
        {
            var id = (IdentityResolver.GetIdentityAsync().Result as UserIdentity)?.Id;

            if (DbContext.Users.Any(u => u.Id == id && u.TeamId == model.TeamId))
                throw new InvalidModelException("User is already on this team.");
        }
    }
}

