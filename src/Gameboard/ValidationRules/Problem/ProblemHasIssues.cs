// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    public class ProblemHasIssues :         
        IValidationRule<ChallengeRestart>
    {
        public ProblemHasIssues(GameboardDbContext dbContext, EnvironmentOptions environmentOptions)
        {
            DbContext = dbContext;
            EnvironmentOptions = environmentOptions;
        }

        EnvironmentOptions EnvironmentOptions { get; }
        GameboardDbContext DbContext { get; }

        public async Task Validate(ChallengeRestart model)
        {
            var problem = DbContext.Problems.Single(t => t.TeamId == model.TeamId && t.ChallengeLinkId == model.ChallengeLinkId);

            var totalMinutes = (DateTime.UtcNow - problem.Start).TotalMinutes;

            var invalidStatus = string.IsNullOrWhiteSpace(problem.Status) || problem.Status.Equals("Registered", StringComparison.InvariantCultureIgnoreCase);

            if (!invalidStatus || totalMinutes < EnvironmentOptions.ResetMinutes)
                throw new InvalidOperationException("Challenge is not in an invalid state.");
        }
    }
}

