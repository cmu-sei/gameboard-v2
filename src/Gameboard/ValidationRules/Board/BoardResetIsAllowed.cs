// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that board allows reset
    /// </summary>
    public class BoardResetIsAllowed :
        IValidationRule<GameEngineSessionReset>,
        IValidationRule<TeamBoardReset>
    {
        IGameFactory GameFactory { get; }

        public BoardResetIsAllowed(IGameFactory gameFactory)
        {
            GameFactory = gameFactory;
        }

        public async Task Validate(GameEngineSessionReset model)
        {
            var board = GameFactory.GetGame().Boards.SingleOrDefault(b => b.Id == model.BoardId);

            if (!board.IsResetAllowed)
                throw new InvalidOperationException("Board reset feature is disabled.");
        }

        public async Task Validate(TeamBoardReset model)
        {
            var board = GameFactory.GetGame().Boards.SingleOrDefault(b => b.Id == model.BoardId);

            if (!board.IsResetAllowed)
                throw new InvalidOperationException("Board reset feature is disabled.");
        }
    }
}

