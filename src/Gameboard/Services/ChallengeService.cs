// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using GameEngine.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    public class ChallengeService : Service
    {
        IGameEngineService GameEngineService { get; }
        public GameboardDbContext DbContext { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="db"></param>
        /// <param name="gameFactory"></param>
        /// <param name="engineService"></param>
        /// <param name="leaderboardOptions"></param>
        public ChallengeService(
            IStackIdentityResolver identityResolver,
            IMapper mapper,
            IValidationHandler validationHandler,
            GameboardDbContext db,
            IGameFactory gameFactory,
            IGameEngineService engineService,            
            LeaderboardOptions leaderboardOptions)
            : base(identityResolver, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            DbContext = db;
            GameEngineService = engineService;
        }

        public async Task<ChallengeProblem> Get(ChallengeRequest model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var game = GameFactory.GetGame();
            var challenge = game.FindChallengeByChallengeLinkId(model.Id);

            string teamId = string.IsNullOrEmpty(model.TeamId)
                ? Identity?.User?.TeamId
                : model.TeamId;

            var problem = string.IsNullOrEmpty(teamId)
                ? null
                : await DbContext.Problems
                    .Include(p => p.Tokens)
                    .Include(p => p.Submissions)
                    .Include("Submissions.User")
                    .Include("Submissions.Tokens")
                    .SingleOrDefaultAsync(p => p.TeamId == teamId && p.ChallengeLinkId == challenge.Id);

            var result = new ChallengeProblem
            {
                Challenge = challenge,
                Problem = Map<ProblemDetail>(problem),
                Board = game.Boards.FindByChallengeLinkId(challenge.Id)
            };

            return result;
        }

        public async Task<ChallengeTagReport> GetChallengeTagReport(ChallengeTagReportDataFilter dataFilter = null)
        {
            var report = new ChallengeTagReport
            {
                Name = Identity.User.Name
            };

            var challengeLinkIdProblems = await DbContext
                .Problems.Where(p => p.TeamId == Identity.User.TeamId && p.Score > 0 && p.End.HasValue)
                .ToDictionaryAsync(p => p.ChallengeLinkId, p => p);

            var challenges = GameFactory.GetGame().FindChallengesByChallengeLinkIds(challengeLinkIdProblems.Keys.ToArray())
                .AsQueryable();

            report.Result = PagedResultFactory.Execute<ChallengeDetail, ChallengeDetail>(
                challenges, 
                dataFilter, 
                Identity, 
                GetMappingOperationOptions());

            foreach (var item in report.Result.Results) {
                var problem = challengeLinkIdProblems[item.Id];

                item.ProblemId = problem.Id;
                item.ProblemScore = problem.Score;
                item.ProblemStatus = problem.Status;
            }

            return report;
        }

        public async Task<PagedResult<ChallengeSpec, ChallengeSpec>> GetAllChallengeSpecs(ChallengeSpecDataFilter dataFilter = null)
        {
            var query = (await GameEngineService.ChallengeSpecs()).AsQueryable();

            return PagedResultFactory.Execute<ChallengeSpec, ChallengeSpec>(query, dataFilter, Identity);
        }

        public async Task<ChallengeSpec> GetChallengeSpec(string name)
        {
            return await GameEngineService.ChallengeSpec(name);
        }

        public async Task<ChallengeSpec> AddChallengeSpec(ChallengeSpec model)
        {
            var cs = await GameEngineService.SaveChallengeSpec(model.Slug, model);
            await GameFactory.Refresh();
            return cs;
        }

        public async Task<ChallengeSpec> UpdateChallengeSpec(string name, ChallengeSpec model)
        {
            var cs = await GameEngineService.SaveChallengeSpec(name, model);
            await GameFactory.Refresh();
            return cs;
        }

        public async Task DeleteChallengeSpec(string name)
        {
            await GameEngineService.DeleteChallengeSpec(name);
            await GameFactory.Refresh();
        }

    }
}

