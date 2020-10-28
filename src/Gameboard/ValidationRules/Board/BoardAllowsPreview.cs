// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that board allows previewing 
    /// </summary>
    public class BoardAllowsPreview : IValidationRule<ChallengeRequest>
    {
        public BoardAllowsPreview(IGameFactory gameFactory)
        {
            GameFactory = gameFactory;
        }

        IGameFactory GameFactory { get; }

        public async Task Validate(ChallengeRequest model)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(model.Id);

            if (!board.IsPreviewAllowed)
                throw new InvalidOperationException("Board disallows preview without enrollment.");
        }
    }
}

