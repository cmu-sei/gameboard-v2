// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    public class TeamDidNotExceedMaxConcurrentProblems : IValidationRule<ChallengeStart>,
        IValidationRule<GamespaceRestart>
    {
        public TeamDidNotExceedMaxConcurrentProblems(GameboardDbContext dbContext, IStackIdentityResolver identityResolver, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
            GameFactory = gameFactory;
        }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        IGameFactory GameFactory { get; }

        GameboardDbContext DbContext { get; }

        public async Task Validate(ChallengeStart model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(model.ChallengeId);
            await Validate(board.Id);
        }

        public async Task Validate(GamespaceRestart model)
        {
            var problem = await DbContext.Problems.FindAsync(model.ProblemId);
            var board = await DbContext.Boards.Where(b => b.Id == problem.BoardId).FirstOrDefaultAsync();
            await Validate(board.Id);
        }

        public async Task Validate(string boardId)
        {
            var board = await DbContext.Boards.FindAsync(boardId);

            if (board.MaxConcurrentProblems > 0)
            {
                var teamId = Identity?.User?.TeamId;

                var incompleteProblemCount = await DbContext.Problems.CountAsync(p =>
                    p.BoardId == board.Id &&
                    p.TeamId == teamId &&
                    p.GamespaceReady &&
                    p.HasGamespace);

                if (incompleteProblemCount >= board.MaxConcurrentProblems)
                    throw new InvalidOperationException("Team at gamespace limit for this board. Finish active challenges or delete a gamespace before starting a new challenge.");
            }
        }
    }
}

