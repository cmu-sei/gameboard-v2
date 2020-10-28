// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.Repositories;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    public class TeamDidNotExceedTimeLimit :
        IValidationRule<ChallengeStart>,
        IValidationRule<BoardStart>,
        IValidationRule<SubmissionCreate>,
        IValidationRule<GamespaceRestart>
    {
        public TeamDidNotExceedTimeLimit(GameboardDbContext dbContext, IStackIdentityResolver identityResolver, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
            GameFactory = gameFactory;
        }

        IGameFactory GameFactory { get; }
        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(ChallengeStart model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(model.ChallengeId);

            await Validate(board);
        }

        public async Task Validate(BoardStart model)
        {
            var game = GameFactory.GetGame();

            var board = game.Boards
                .SingleOrDefault(b => b.Id == model.Id);

            await Validate(board);
        }

        public async Task Validate(SubmissionCreate model)
        {
            var challengeId = DbContext.Problems.FirstOrDefault(p => p.Id == model.ProblemId)?.ChallengeLinkId;

            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(challengeId);

            await Validate(board);
        }

        public async Task Validate(GamespaceRestart model)
        {
            var challengeId = DbContext.Problems.FirstOrDefault(p => p.Id == model.ProblemId)?.ChallengeLinkId;

            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(challengeId);

            await Validate(board);
        }

        async Task Validate(BoardDetail board)
        {
            var teamId = Identity?.User?.TeamId;

            var teamBoard = await DbContext.TeamBoards.SingleOrDefaultAsync(tb => tb.TeamId == teamId && tb.BoardId == board.Id);

            if (teamBoard != null)
            {
                var maxMinutes = teamBoard.OverrideMaxMinutes ?? board.MaxMinutes;

                if (maxMinutes > 0 && teamBoard.Start.AddMinutes(maxMinutes) < DateTime.UtcNow)
                    throw new InvalidOperationException("Maximum time limit has been exceeded.");
            }
        }
    }
}

