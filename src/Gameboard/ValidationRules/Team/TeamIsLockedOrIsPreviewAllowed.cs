// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    public class TeamIsLockedOrIsPreviewAllowed : IValidationRule<ChallengeRequest>
    {
        public TeamIsLockedOrIsPreviewAllowed(GameboardDbContext dbContext, IGameFactory gameFactory, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IGameFactory GameFactory { get; }

        IStackIdentityResolver IdentityResolver { get; }

        User User { get { return Identity?.User; } }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(ChallengeRequest model)
        {
            if (User != null && (User.IsModerator || User.IsObserver))
                return;

            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(model.Id);

            if (!board.IsPreviewAllowed)
            {
                if (string.IsNullOrEmpty(User?.TeamId) ||
                    await DbContext.Teams.AnyAsync(t => t.Id == User.TeamId && !t.IsLocked))
                    throw new InvalidOperationException("Enrollment must be locked to view challenges.");
            }

        }
    }
}

