// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team is unlocked
    /// </summary>
    public class TeamIsUnlocked :
        IValidationRule<TeamUserUpdate>,
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamUserDelete>,
        IValidationRule<TeamLock>,
        IValidationRule<TeamInviteCode>,
        IValidationRule<TeamUserLeave>
    {
        public TeamIsUnlocked(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
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

        public async Task Validate(TeamUserUpdate model)
        {
            var inviteCode = model.InviteCode.ToLower().Trim();

            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.InviteCode.ToLower() == inviteCode && !t.IsLocked);

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }

        public async Task Validate(TeamUpdate model)
        {
            Team team = null;

            if (User.IsModerator)
            {
                team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.Id);
            }
            else
            {
                team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.Id && !t.IsLocked);
            }

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }

        public async Task Validate(TeamLock model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId && !t.IsLocked);

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }

        public async Task Validate(TeamUserDelete model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId && !t.IsLocked);

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }

        public async Task Validate(TeamInviteCode model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId && !t.IsLocked);

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }

        public async Task Validate(TeamUserLeave model)
        {
            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.Id == model.TeamId && !t.IsLocked);

            if (team == null)
                throw new InvalidModelException("Unlocked team was not found.");
        }
    }
}

