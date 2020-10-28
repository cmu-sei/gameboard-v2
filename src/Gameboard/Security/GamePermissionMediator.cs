// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;

namespace Gameboard.Security
{
    /// <summary>
    /// game permission mediator
    /// </summary>
    /// <remarks>determine permissions on database actions for games by identity</remarks>
    public class GamePermissionMediator : PermissionMediator<Game>, IPermissionMediator<Game>
    {
        public GamePermissionMediator(IStackIdentityResolver identityResolver)
            : base(identityResolver) { }

        public override bool CanUpdate(Game entity)
        {
            if (IsGameDesigner)
                return true;

            return false;
        }

        public override bool CanDelete(Game entity)
        {
            if (IsGameDesigner)
                return true;

            return false;
        }
    }
}

