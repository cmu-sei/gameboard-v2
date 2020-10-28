// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validates that team is not at or over max team size
    /// </summary>
    public class MaxTeamSizeIsValid :
        IValidationRule<TeamUserUpdate>,
        IValidationRule<BoardStart>,
        IValidationRule<TeamLock>
    {
        public MaxTeamSizeIsValid(GameboardDbContext dbContext, IStackIdentityResolver identityResolver, IGameFactory gameFactory)
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

        public async Task Validate(TeamUserUpdate model)
        {
            var inviteCode = model.InviteCode.ToLower().Trim();

            var team = await DbContext.Teams
                .Include(t => t.Users)
                .SingleOrDefaultAsync(t => t.InviteCode.ToLower() == inviteCode);

            if (team == null)
                throw new EntityNotFoundException("No team found with this invite code.");

            var game = GameFactory.GetGame();

            if (game.MaxTeamSize > 0 && team.Users.Count() >= game.MaxTeamSize)
                throw new InvalidModelException("Team is at maximum size.");
        }

        public async Task Validate(BoardStart model)
        {
            var teamId = Identity?.User?.TeamId;

            var team = await DbContext.Teams
                .Include(t => t.Users)
                .SingleOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new EntityNotFoundException("No team found with this ID.");

            var game = GameFactory.GetGame();

            if (game.MaxTeamSize > 0 && team.Users.Count() > game.MaxTeamSize)
                throw new InvalidModelException("Team has exceeded maximum size.");
        }

        public async Task Validate(TeamLock model)
        {
            var team = await DbContext.Teams
                .Include(t => t.Users)
                .SingleOrDefaultAsync(t => t.Id == model.TeamId);

            if (team == null)
                throw new EntityNotFoundException("No team found with this ID.");

            var game = GameFactory.GetGame();

            if (game.MaxTeamSize > 0 && team.Users.Count() > game.MaxTeamSize)
                throw new InvalidModelException("Team has exceeded maximum size.");
        }
    }
}

