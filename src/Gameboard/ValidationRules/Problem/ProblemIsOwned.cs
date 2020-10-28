// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that problem team owner is identity
    /// </summary>
    public class ProblemIsOwned : 
        IValidationRule<SubmissionCreate>,        
        IValidationRule<GamespaceRestart>,
        IValidationRule<GamespaceDelete>
    {
       public ProblemIsOwned(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(SubmissionCreate model)
        {
            await Validate(model.ProblemId);
        }

        public async Task Validate(string problemId)
        {
            var problemExists = await DbContext.Problems.AnyAsync(t => t.Id == problemId && t.TeamId == Identity.User.TeamId);

            if (!problemExists)
            {
                problemExists = await DbContext.Problems.AnyAsync(t => t.SharedId == problemId && t.TeamId == Identity.User.TeamId);
            }

            if (!problemExists)
                throw new InvalidOperationException("Challenge does not belong to this team.");
        }

        public async Task Validate(GamespaceDelete model)
        {
            await Validate(model.Id);
        }

        public async Task Validate(GamespaceRestart model)
        {
            await Validate(model.ProblemId);
        }
    }
}

