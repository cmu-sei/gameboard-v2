// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;

namespace Gameboard.Security
{
    /// <summary>
    /// user permission mediator
    /// </summary>
    /// <remarks>determine permissions on database actions for users by identity</remarks>
    public class UserPermissionMediator : PermissionMediator<User>, IPermissionMediator<User>
    {
        public UserPermissionMediator(IStackIdentityResolver identityResolver)
            : base(identityResolver) { }

        public override bool CanUpdate(User entity)
        {
            if (IsModerator)
                return true;

            if (User.Id == entity.Id)
                return true;

            return false;
        }

        public override bool CanDelete(User entity)
        {
            if (IsModerator)
                return true;

            return false;
        }
    }
}

