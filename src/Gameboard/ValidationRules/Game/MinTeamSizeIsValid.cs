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
    /// validate that team is at or above max team size
    /// </summary>
    public class MinTeamSizeIsValid :
        IValidationRule<BoardStart>,
        IValidationRule<TeamLock>
    {
        public MinTeamSizeIsValid(GameboardDbContext dbContext, IGameFactory gameFactory, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
            IdentityResolver = identityResolver;
        }

        IGameFactory GameFactory { get; }

        IStackIdentityResolver IdentityResolver { get; }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }
        GameboardDbContext DbContext { get; }

        public async Task Validate(BoardStart model)
        {
            await Validate(Identity?.User?.TeamId);
        }

        public async Task Validate(TeamLock model)
        {
            await Validate(model.TeamId);
        }

        async Task Validate(string teamId)
        {
            var team = await DbContext.Teams
                .Include(t => t.Users)
                .SingleOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                throw new EntityNotFoundException("No team found with this ID.");

            var game = GameFactory.GetGame();

            if (game.MinTeamSize > 0 && team.Users.Count() < game.MinTeamSize)
                throw new InvalidModelException("Team has not reached minimum team size.");
        }
    }
}

