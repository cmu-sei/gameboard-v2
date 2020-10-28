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
    /// validate that team is locked
    /// </summary>
    public class TeamIsLocked :
        IValidationRule<ChallengeStart>,
        IValidationRule<BoardStart>,
        IValidationRule<SubmissionCreate>
    {
        public TeamIsLocked(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
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

        public async Task Validate(ChallengeStart model)
        {
            await Validate();
        }

        public async Task Validate(BoardStart model)
        {
            await Validate();
        }

        public async Task Validate(SubmissionCreate model)
        {
            await Validate();
        }

        async Task Validate()
        {
            var teamId = Identity?.User?.TeamId;

            if (string.IsNullOrEmpty(teamId))
                throw new InvalidOperationException("Please enroll first.");

            if (await DbContext.Teams.AnyAsync(t => t.Id == teamId && !t.IsLocked))
                throw new InvalidOperationException("Enrollment must be locked to start the competition");
        }
    }
}

