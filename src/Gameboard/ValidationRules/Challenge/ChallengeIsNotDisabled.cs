// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that challenge has not been disabled
    /// </summary>
    public class ChallengeIsNotDisabled :        
        IValidationRule<ChallengeStart>,
        IValidationRule<SubmissionCreate>
    {
        public ChallengeIsNotDisabled(IGameFactory gameFactory, GameboardDbContext db)
        {
            GameFactory = gameFactory;
            DbContext = db;
        }

        IGameFactory GameFactory { get; }
        GameboardDbContext DbContext { get; }

        public async Task Validate(ChallengeStart model)
        {
            await Validate(model.ChallengeId);
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

            bool disabledCoordinate(CoordinateDetail c) => c.ChallengeLink != null && c.ChallengeLink.Id == challengeLinkId && c.IsDisabled;
            bool disabledQuestion(QuestionDetail q) => q.ChallengeLink != null && q.ChallengeLink.Id == challengeLinkId && q.IsDisabled;

            if ((board.BoardType == BoardType.Map && board.Maps.SelectMany(c => c.Coordinates).Any(disabledCoordinate)) ||
                (board.BoardType == BoardType.Trivia && board.Categories.SelectMany(c => c.Questions).Any(disabledQuestion)))
                throw new InvalidModelException("Challenge is disabled and cannot be started");
        }
    }
}

