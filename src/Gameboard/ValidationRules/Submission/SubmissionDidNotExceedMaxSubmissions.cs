// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Microsoft.EntityFrameworkCore;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;
using Gameboard.Repositories;
using System.Linq;
using Gameboard.Services;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that submissions are less than <see cref="Board.MaxSubmissions"/>
    /// </summary>
    public class SubmissionDidNotExceedMaxSubmissions : IValidationRule<SubmissionCreate>
    {
        public SubmissionDidNotExceedMaxSubmissions(GameboardDbContext dbContext, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
        }

        GameboardDbContext DbContext { get; }
        IGameFactory GameFactory { get; }

        public async Task Validate(SubmissionCreate model)
        {
            var problem = await DbContext.Problems.FindAsync(model.ProblemId);

            var game = GameFactory.GetGame();

            var challenge = game.GetChallenges().FirstOrDefault(c => c.Id == problem.ChallengeLinkId);
            
            var board = game.Boards.FirstOrDefault(b => b.Id == problem.BoardId);

            var submissions = DbContext.Submissions.AsNoTracking()
                .Include(s => s.Tokens)
                .Where(s => s.ProblemId == model.ProblemId)
                .OrderBy(s => s.Timestamp)
                .ToList();

            var count = SubmissionService.CalculateSubmissionCount(board, challenge, submissions);

            if (board.MaxSubmissions > 0 && count >= board.MaxSubmissions)
                throw new InvalidOperationException("Maximum submissions reached.");
        }
    }
}

