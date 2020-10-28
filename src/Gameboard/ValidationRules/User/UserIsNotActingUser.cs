// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Identity;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user id does not match identity
    /// </summary>
    public class UserIsNotActingUser : IValidationRule<UserReset>
    {
        public UserIsNotActingUser(IStackIdentityResolver identityResolver)
        {
            IdentityResolver = identityResolver;
        }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(UserReset model)
        {
            var id = (IdentityResolver.GetIdentityAsync().Result as UserIdentity)?.Id;

            if (model.Id == id)
                throw new InvalidModelException("You cannot reset your own user.");
        }
    }
}

