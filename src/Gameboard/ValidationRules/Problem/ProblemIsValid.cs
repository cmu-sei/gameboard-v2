// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Microsoft.EntityFrameworkCore;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that problem exists
    /// </summary>
    public class ProblemIsValid : 
        IValidationRule<SubmissionCreate>,
        IValidationRule<GamespaceRestart>,
        IValidationRule<ChallengeReset>,
        IValidationRule<ChallengeRestart>
    {
        public ProblemIsValid(GameboardDbContext dbContext)
        {
            DbContext = dbContext;
        }

        GameboardDbContext DbContext { get; }

        public async Task Validate(SubmissionCreate model)
        {
            await Validate(model.ProblemId);
        }

        public async Task Validate(GamespaceRestart model)
        {
            await Validate(model.ProblemId);
        }

        public async Task Validate(ChallengeReset model)
        {
            await Validate(model.TeamId, model.ChallengeLinkId);
        }

        public async Task Validate(ChallengeRestart model)
        {
            await Validate(model.TeamId, model.ChallengeLinkId);
        }

        async Task Validate(string problemId)
        {
            if (!await DbContext.Problems.AnyAsync(t => t.Id == problemId))
                throw new InvalidOperationException("Challenge has not been started.");
        }

        async Task Validate(string teamId, string challengeLinkId)
        {
            if (!await DbContext.Problems.AnyAsync(t => t.TeamId == teamId && t.ChallengeLinkId == challengeLinkId))
                throw new InvalidOperationException("Challenge has not been started.");
        }
    }
}

