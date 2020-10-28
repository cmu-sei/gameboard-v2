// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ValidationRules
{
    /// <summary>
    /// validate that team name is not already taken
    /// </summary>
    public class TeamNameIsAvailable :
        IValidationRule<TeamUpdate>,
        IValidationRule<TeamCreate>
    {
        public TeamNameIsAvailable(GameboardDbContext dbContext)
        {
            DbContext = dbContext;
        }

        GameboardDbContext DbContext { get; }

        public async Task Validate(TeamUpdate model)
        {
            if (DbContext.Teams.Any(t => t.Name.ToLower() == model.Name.ToLower() && t.Id != model.Id))
                throw new EntityDuplicateException("Team name is not available.");
        }

        public async Task Validate(TeamCreate model)
        {
            if (DbContext.Teams.Any(t => t.Name.ToLower() == model.Name.ToLower()))
                throw new EntityDuplicateException("Team name is not available.");
        }
    }
}

