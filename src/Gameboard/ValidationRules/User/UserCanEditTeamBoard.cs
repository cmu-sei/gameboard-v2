// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user can edit team board
    /// </summary>
    public class UserCanEditTeamBoard : IValidationRule<TeamBoardUpdate>
    {
        public UserCanEditTeamBoard(IStackIdentityResolver identityResolver)
        {
            IdentityResolver = identityResolver;
        }

        IStackIdentityResolver IdentityResolver { get; }

        User User { get { return Identity?.User; } }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(TeamBoardUpdate model)
        {
            if (!User.IsModerator)
                throw new EntityPermissionException("User is not a moderator.");
        }
    }
}

