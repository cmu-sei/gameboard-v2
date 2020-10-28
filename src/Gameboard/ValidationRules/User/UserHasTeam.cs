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
    /// validate that user is a member of a team
    /// </summary>
    public class UserHasTeam :
        IValidationRule<ProblemConsole>,
        IValidationRule<ProblemConsoleAction>,
        IValidationRule<ChallengeStart>
    {
        public UserHasTeam(IStackIdentityResolver identityResolver)
        {
            IdentityResolver = identityResolver;
        }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(ChallengeStart model)
        {
            await Validate();
        }

        public async Task Validate(ProblemConsole model)
        {
            await Validate();
        }

        public async Task Validate(ProblemConsoleAction model)
        {
            await Validate();
        }

        async Task Validate()
        {
            if (string.IsNullOrWhiteSpace(Identity?.User?.TeamId))
                throw new InvalidModelException("User has no team.");
        }
    }
}

