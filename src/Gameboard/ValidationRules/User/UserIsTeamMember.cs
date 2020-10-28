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
    /// validate that user is member of team
    /// </summary>
    public class UserIsTeamMember :
        IValidationRule<TeamUserDelete>,
        IValidationRule<TeamUserLeave>,
        IValidationRule<ProblemConsole>,
        IValidationRule<ProblemConsoleAction>
    {
        public UserIsTeamMember(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        User User { get { return Identity?.User; } }

        UserIdentity Identity
        {
            get { return IdentityResolver.GetIdentityAsync().Result as UserIdentity; }
        }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(TeamUserDelete model)
        {
            if (!DbContext.Users.Any(u => u.Id == model.UserId && u.TeamId == model.TeamId))
                throw new InvalidModelException("User is not on this team.");
        }

        public async Task Validate(TeamUserLeave model)
        {
            var id = (IdentityResolver.GetIdentityAsync().Result as UserIdentity)?.Id;

            if (!DbContext.Users.Any(u => u.Id == id && u.TeamId == model.TeamId))
                throw new InvalidModelException("User is not on this team.");
        }

        public async Task Validate(ProblemConsole model)
        {
            if (User.IsModerator || User.IsObserver)
                return;

            var problem = await DbContext.Problems.SingleOrDefaultAsync(p => p.Id == model.Id);

            if (problem == null)
            {
                problem = await DbContext.Problems.FirstOrDefaultAsync(p => p.SharedId == model.Id);
            }

            if (problem == null || !DbContext.Users.Any(u => u.Id == User.Id && u.TeamId == problem.TeamId))
                throw new InvalidModelException("User is not on this team.");
        }

        public async Task Validate(ProblemConsoleAction model)
        {
            if (User.IsModerator || User.IsObserver)
                return;

            var problem = await DbContext.Problems.SingleOrDefaultAsync(p => p.Id == model.Id);
            if (problem == null || !DbContext.Users.Any(u => u.Id == User.Id && u.TeamId == problem.TeamId))
                throw new InvalidModelException("User is not on this team.");
        }
    }
}

