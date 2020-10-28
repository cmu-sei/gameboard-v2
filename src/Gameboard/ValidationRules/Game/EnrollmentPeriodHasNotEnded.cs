// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Identity;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that game enrollment period has not ended
    /// </summary>
    public class EnrollmentPeriodHasNotEnded :
        IValidationRule<UserEdit>,
        IValidationRule<TeamCreate>,
        IValidationRule<TeamInviteCode>,
        IValidationRule<TeamLock>,
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamUserDelete>,
        IValidationRule<TeamUserLeave>,
        IValidationRule<TeamUserUpdate>
    {
        public EnrollmentPeriodHasNotEnded(IStackIdentityResolver identityResolver, IGameFactory gameFactory)
        {
            IdentityResolver = identityResolver;
            GameFactory = gameFactory;
        }

        IGameFactory GameFactory { get; }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(UserEdit model) { await Validate(); }

        public async Task Validate(TeamCreate model) { await Validate(); }

        public async Task Validate(TeamInviteCode model) { await Validate(); }

        public async Task Validate(TeamLock model) { await Validate(); }

        public async Task Validate(TeamUpdate model) { await Validate(); }

        public async Task Validate(TeamUserDelete model) { await Validate(); }

        public async Task Validate(TeamUserLeave model) { await Validate(); }

        public async Task Validate(TeamUserUpdate model) { await Validate(); }

        async Task Validate()
        {
            var game = GameFactory.GetGame();
            var enrollmentEndsAt = game.EnrollmentEndsAt;

            var identity = await IdentityResolver.GetIdentityAsync() as UserIdentity;

            if (!identity.User.IsModerator && enrollmentEndsAt.HasValue && enrollmentEndsAt < DateTime.UtcNow)
                throw new InvalidModelException("The enrollment period has ended.");
        }
    }
}

