// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Stack.Http.Identity;
using System.Threading.Tasks;

namespace Gameboard.Tests
{
    public class TestIdentityResolver : IStackIdentityResolver
    {
        User _user;

        public TestIdentityResolver(User user)
        {
            _user = user;
        }

        public async Task<IStackIdentity> GetIdentityAsync()
        {
            if (_user == null)
                return null;

            return new UserIdentity
            {
                Id = _user.Id,
                User = _user
            };
        }
    }
}
