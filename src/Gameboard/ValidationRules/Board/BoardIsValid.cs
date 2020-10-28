// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that board request is valid
    /// </summary>
    public class BoardIsValid :
        IValidationRule<BoardRequest>
    {
        public BoardIsValid(IGameFactory gameFactory)
        {            
            GameFactory = gameFactory;
        }
        IGameFactory GameFactory { get; }

        public async Task Validate(BoardRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
                throw new InvalidModelException($"Board ID '{model.Id}' is invalid.");

            var game = GameFactory.GetGame();

            if (!game.Boards.Any(c => c.Id == model.Id))
                throw new EntityNotFoundException($"Board '{model.Id}' was not found.");
        }
    }
}

