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
    /// validate that problem does not have an end date
    /// </summary>
    public class ProblemIsActive : IValidationRule<SubmissionCreate>,
        IValidationRule<GamespaceRestart>
    {
        public ProblemIsActive(GameboardDbContext dbContext)
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

        public async Task Validate(string problemId)
        {
            if (await DbContext.Submissions.AnyAsync(t => t.Id == problemId && t.Problem.End != null))
                throw new InvalidOperationException("Challenge is already complete.");
        }


    }
}

