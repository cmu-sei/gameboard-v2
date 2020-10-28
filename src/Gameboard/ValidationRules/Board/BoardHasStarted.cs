// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that board has started
    /// </summary>
    public class BoardHasStarted :
        IValidationRule<BoardRequest>,
        IValidationRule<ChallengeRequest>,
        IValidationRule<ChallengeStart>,
        IValidationRule<BoardStart>,
        IValidationRule<SubmissionCreate>
    {
        public BoardHasStarted(GameboardDbContext dbContext, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
        }

        GameboardDbContext DbContext { get; }
        IGameFactory GameFactory { get; }

        public async Task Validate(BoardRequest model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == model.Id);

            if (!board.HasStarted())
                throw new InvalidOperationException("This board is not started.");
        }

        public async Task Validate(ChallengeRequest model)
        {
            await Validate(model.Id);
        }

        public async Task Validate(ChallengeStart model)
        {
            await Validate(model.ChallengeId);
        }

        public async Task Validate(BoardStart model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == model.Id);

            if (!board.HasStarted())
                throw new InvalidOperationException("This board is not started.");
        }

        public async Task Validate(SubmissionCreate model)
        {
            var problem = await DbContext.Problems.SingleOrDefaultAsync(p => p.Id == model.ProblemId);

            await Validate(problem.ChallengeLinkId);
        }

        async Task Validate(string challengeLinkId)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(challengeLinkId);

            if (!board.HasStarted())
                throw new InvalidOperationException("This board is not started.");
        }
    }
}

