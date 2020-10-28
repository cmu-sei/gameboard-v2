// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that environment is in test mode
    /// </summary>
    public class IsTestMode :
        IValidationRule<ChallengeReset>,        
        IValidationRule<UserReset>,        
        IValidationRule<TeamReset>
    {
        public IsTestMode(EnvironmentOptions environmentOptions)
        {
            EnvironmentOptions = environmentOptions;
        }
        EnvironmentOptions EnvironmentOptions { get; }

        public async Task Validate(ChallengeReset model)
        {
            await Validate();
        }

        public async Task Validate(UserReset model)
        {
            await Validate();
        }

        public async Task Validate(TeamReset model)
        {
            await Validate();
        }

        async Task Validate()
        {
            if (EnvironmentOptions.Mode != EnvironmentMode.Test)
                throw new InvalidModelException("Game is not in test mode.");
        }
    }
}

