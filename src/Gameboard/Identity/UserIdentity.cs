// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using System.Security.Claims;

namespace Gameboard.Identity
{
    /// <summary>
    /// gameboard user identity
    /// </summary>
    public class UserIdentity : StackIdentity, IStackIdentity
    {
        public User User { get; set; }

        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}

