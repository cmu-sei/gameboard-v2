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
    /// <summary>
    /// validate that team has started team board
    /// </summary>
    public class TeamHasTeamBoard :
        IValidationRule<ChallengeStart>,
        IValidationRule<GameEngineSessionReset>,
        IValidationRule<TeamBoardReset>
    {
        public TeamHasTeamBoard(GameboardDbContext dbContext, IStackIdentityResolver identityResolver, IGameFactory gameFactory)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
            GameFactory = gameFactory;
        }

        IStackIdentityResolver IdentityResolver { get; }

        GameboardDbContext DbContext { get; }

        IGameFactory GameFactory { get; }

        public async Task Validate(ChallengeStart model)
        {
            var game = GameFactory.GetGame();

            var board = game.Boards.FindByChallengeLinkId(model.ChallengeId);

            var teamId = (await IdentityResolver.GetIdentityAsync() as UserIdentity)?.User?.TeamId;

            if (board.MaxMinutes > 0)
                await Validate(board.Id, teamId);
        }

        async Task Validate(string boardId, string teamId)
        {
            if (! await DbContext.TeamBoards.AnyAsync(tb => tb.TeamId == teamId && tb.BoardId == boardId))
                throw new InvalidOperationException("Team session is not started.");
        }

        public async Task Validate(GameEngineSessionReset model)
        {
            await Validate(model.BoardId, model.TeamId);
        }

        public async Task Validate(TeamBoardReset model)
        {
            await Validate(model.BoardId, model.TeamId);
        }
    }
}

