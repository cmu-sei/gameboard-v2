// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user is team owner or moderator
    /// </summary>
    public class UserCanEditTeam :
        IValidationRule<TeamLock>,
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamInviteCode>,
        IValidationRule<TeamUserDelete>,
        IValidationRule<BoardStart>,
        IValidationRule<GameEngineSessionReset>,
        IValidationRule<TeamBoardReset>,
        IValidationRule<ChallengeReset>,
        IValidationRule<ChallengeRestart>
    {
        public UserCanEditTeam(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        string IdentityId { get { return Identity?.Id; } }

        User User { get { return Identity?.User; } }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        public async Task Validate(TeamLock model)
        {
            await Validate(model.TeamId);
        }

        public async Task Validate(TeamUpdate model)
        {
            await Validate(model.Id);
        }

        public async Task Validate(TeamUserDelete model)
        {
            await Validate(model.TeamId);
        }

        public async Task Validate(TeamInviteCode model)
        {
            await Validate(model.TeamId);
        }

        public async Task Validate(BoardStart model)
        {
            var teamId = Identity?.User?.TeamId;

            await Validate(teamId);
        }

        public async Task Validate(GameEngineSessionReset model)
        {
            var teamId = Identity?.User?.TeamId;

            await Validate(teamId);
        }

        public async Task Validate(TeamBoardReset model)
        {
            await Validate(model.TeamId);
        }

        public async Task Validate(ChallengeReset model)
        {
            await Validate(model.TeamId);
        }

        public async Task Validate(ChallengeRestart model)
        {
            await Validate(model.TeamId);
        }

        async Task Validate(string teamId)
        {
            if (!Identity.User.IsModerator && !DbContext.Teams.Any(t => t.Id == teamId && t.OwnerUserId == IdentityId))
                throw new EntityPermissionException("User is not team leader.");
        }
    }
}

