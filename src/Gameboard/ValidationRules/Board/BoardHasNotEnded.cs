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
    /// validates that board has not ended
    /// </summary>
    public class BoardHasNotEnded :
        IValidationRule<ChallengeStart>,
        IValidationRule<BoardStart>,
        IValidationRule<GameEngineSessionReset>,
        IValidationRule<TeamBoardReset>,
        IValidationRule<ChallengeReset>,
        IValidationRule<SubmissionCreate>,
        IValidationRule<ChallengeRestart>
    {
        public BoardHasNotEnded(GameboardDbContext dbContext, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
        }

        GameboardDbContext DbContext { get; }

        IGameFactory GameFactory { get; }

        public async Task Validate(ChallengeStart model)
        {
            await ValidateByChallengeLinkId(model.ChallengeId);
        }

        public async Task Validate(BoardStart model)
        {
            await ValidateByBoardId(model.Id);
        }

        public async Task Validate(SubmissionCreate model)
        {
            var problem = await DbContext.Problems.SingleOrDefaultAsync(p => p.Id == model.ProblemId);

            await ValidateByChallengeLinkId(problem.ChallengeLinkId);
        }

        public async Task Validate(GameEngineSessionReset model)
        {
            await ValidateByBoardId(model.BoardId);
        }

        public async Task Validate(TeamBoardReset model)
        {
            await ValidateByBoardId(model.BoardId);
        }

        public async Task Validate(ChallengeReset model)
        {
            await ValidateByChallengeLinkId(model.ChallengeLinkId);
        }

        public async Task Validate(ChallengeRestart model)
        {
            await ValidateByChallengeLinkId(model.ChallengeLinkId);
        }

        async Task ValidateByChallengeLinkId(string challengeLinkId)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(challengeLinkId);

            if (!board.HasNotEnded())
                throw new InvalidOperationException("This round has ended.");
        }

        async Task ValidateByBoardId(string boardId)
        {
            var game = GameFactory.GetGame();

            var board = game.Boards.SingleOrDefault(b => b.Id == boardId);

            if (!board.HasNotEnded())
                throw new InvalidOperationException("This round has ended.");
        }
    }
}

