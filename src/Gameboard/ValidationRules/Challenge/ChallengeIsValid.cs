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
    /// validates challenge request and start challenge is valid
    /// </summary>
    public class ChallengeIsValid :
        IValidationRule<ChallengeRequest>,
        IValidationRule<ChallengeStart>
    {
        public ChallengeIsValid(IGameFactory gameFactory)
        {
            GameFactory = gameFactory;
        }

        IGameFactory GameFactory { get; }

        public async Task Validate(ChallengeRequest model)
        {
            await Validate(model.Id);
        }

        public async Task Validate(ChallengeStart model)
        {
            await Validate(model.ChallengeId);
        }

        async Task Validate(string challengeId)
        {
            if (string.IsNullOrWhiteSpace(challengeId))
                throw new InvalidModelException($"Challenge ID '{challengeId}' is invalid.");

            var game = GameFactory.GetGame();
            var challenges = game.GetChallenges();

            var challenge = challenges.SingleOrDefault(c => c.Id == challengeId);

            if (challenge == null)
                throw new EntityNotFoundException($"Challenge '{challengeId}' was not found.");
        }
    }
}

