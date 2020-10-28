// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Validation.Rules;
using System;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.ValidationRules
{
    public class TeamHasNoSession : IValidationRule<BoardStart>
    {
        public TeamHasNoSession(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        IStackIdentityResolver IdentityResolver { get; }

        GameboardDbContext DbContext { get; }

        public async Task Validate(BoardStart model)
        {
            var teamId = (await IdentityResolver.GetIdentityAsync() as UserIdentity)?.User?.TeamId;

            if (await DbContext.TeamBoards.AnyAsync(tb => tb.TeamId == teamId && tb.BoardId == model.Id))
                throw new InvalidOperationException("Team session already started.");
        }

    }
}

