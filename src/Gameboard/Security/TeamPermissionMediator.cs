// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;

namespace Gameboard.Security
{
    /// <summary>
    /// team permission mediator
    /// </summary>
    /// <remarks>determine permissions on database actions for teams by identity</remarks>
    public class TeamPermissionMediator : PermissionMediator<Team>, IPermissionMediator<Team>
    {
        public TeamPermissionMediator(IStackIdentityResolver identityResolver)
            : base(identityResolver) { }

        public override bool CanUpdate(Team entity)
        {
            if (IsModerator)
                return true;

            if (User.TeamId == entity.Id && entity.OwnerUserId == User.Id)
                return true;

            return false;
        }

        public override bool CanDelete(Team entity)
        {
            if (IsModerator)
                return true;

            return false;
        }
    }
}

