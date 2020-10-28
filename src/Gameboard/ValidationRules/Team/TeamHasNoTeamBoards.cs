// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that user team has no team boards    
    /// </summary>
    public class TeamHasNoTeamBoards :        
        IValidationRule<UserReset>

    {
        GameboardDbContext DbContext { get; }

        public TeamHasNoTeamBoards(GameboardDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task Validate(UserReset model)
        {
            var user = DbContext.Users
                .Include(u => u.Team)
                    .ThenInclude(t => t.TeamBoards)
                .SingleOrDefault(u => u.Id == model.Id);

            if (user == null)
                throw new EntityNotFoundException($"User id '{model.Id}' was not found.");

            if (user.Team != null && user.Team.TeamBoards.Any())
                throw new InvalidOperationException("Team has started boards.");

            return;
        }
    }
}

