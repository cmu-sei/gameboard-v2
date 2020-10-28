// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;
using Gameboard.ViewModels;
using Gameboard.Repositories;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// checks if the team a user is associated with has the badges
    /// the board they are trying to start requires
    /// </summary>
    public class TeamHasBoardPrerequisites :
        IValidationRule<BoardStart>
    {
        IStackIdentityResolver IdentityResolver { get; }
        GameboardDbContext DbContext { get; }
        IGameFactory GameFactory { get; }

        public TeamHasBoardPrerequisites(GameboardDbContext dbContext, IGameFactory gameFactory, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
            IdentityResolver = identityResolver;
        }

        public async Task Validate(BoardStart model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == model.Id);

            if (string.IsNullOrWhiteSpace(board.RequiredBadges))
                return;

            var identity = (await IdentityResolver.GetIdentityAsync()) as UserIdentity;
            var badges = (await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == identity.User.TeamId)).Badges.ToBadgeArray();
            var prerequisites = board.RequiredBadges.ToBadgeArray();

            if (!badges.ContainsAll(prerequisites))
                throw new InvalidOperationException("Team does not meet board prerequisites.");

            return;
        }
    }
}

