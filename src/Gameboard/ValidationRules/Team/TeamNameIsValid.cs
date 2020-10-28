// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team name is not empty
    /// </summary>
    public class TeamNameIsValid :
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamCreate>
    {
        public async Task Validate(TeamUpdate model)
        {
            if (string.IsNullOrWhiteSpace((model.Name ?? "").Trim()))
                throw new InvalidModelException("Team name is required.");
        }

        public async Task Validate(TeamCreate model)
        {
            if (string.IsNullOrWhiteSpace((model.Name ?? "").Trim()))
                throw new InvalidModelException("Team name is required.");
        }
    }
}

