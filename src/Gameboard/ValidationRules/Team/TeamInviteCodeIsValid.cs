// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team invite code matches <see cref="Team.InviteCode"/>
    /// </summary>
    public class TeamInviteCodeIsValid : IValidationRule<TeamUserUpdate>
    {
        public TeamInviteCodeIsValid(GameboardDbContext dbContext)
        {
            DbContext = dbContext;
        }

        GameboardDbContext DbContext { get; }

        public async Task Validate(TeamUserUpdate model)
        {
            var inviteCode = model.InviteCode.ToLower().Trim();

            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.InviteCode.ToLower() == inviteCode);

            if (team == null)
                throw new EntityNotFoundException("Team with this code was not found.");
        }
    }
}

