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
    /// validate that problem has not been started
    /// </summary>
    public class ProblemHasNotStarted : IValidationRule<ChallengeStart>
    {
        public ProblemHasNotStarted(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        GameboardDbContext DbContext { get; }

        public async Task Validate(ChallengeStart model)
        {
            var teamId = Identity?.User?.TeamId;

            if (await DbContext.Problems.AnyAsync(p => p.ChallengeLinkId == model.ChallengeId && p.TeamId == teamId))
                throw new InvalidOperationException("Challenge already started.");
        }
    }
}

