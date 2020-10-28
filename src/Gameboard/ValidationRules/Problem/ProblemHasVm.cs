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
    /// <summary>
    /// validate that problem have vm id
    /// </summary>
    public class ProblemHasVm : IValidationRule<ProblemConsole>,
        IValidationRule<ProblemConsoleAction>
    {
        public ProblemHasVm(GameboardDbContext dbContext, IStackIdentityResolver identityResolver)
        {
            DbContext = dbContext;
            IdentityResolver = identityResolver;
        }

        GameboardDbContext DbContext { get; }

        IStackIdentityResolver IdentityResolver { get; }

        public async Task Validate(ProblemConsole model)
        {
            var identity = IdentityResolver.GetIdentityAsync().Result as UserIdentity;

            // the Id on the model can be ProblemId or SharedId so we need to check for both
            var problemExists = await DbContext.Problems.AnyAsync(p => (p.Id == model.Id || p.SharedId == model.Id) && p.GamespaceText.Contains(model.VmId));

            if (!problemExists)
            {
                throw new InvalidOperationException("");
            }    
        }

        public async Task Validate(ProblemConsoleAction model)
        {
            var identity = IdentityResolver.GetIdentityAsync().Result as UserIdentity;

            if (! await DbContext.Problems.AnyAsync(t => t.Id == model.Id && t.GamespaceText.Contains(model.VmId)))
                throw new InvalidOperationException("");
        }
    }
}

