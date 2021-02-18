// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// problem service
    /// </summary>
    public class ProblemService : Service<IProblemRepository, Data.Problem>
    {
        IGameEngineService EngineService { get; }

        EnvironmentOptions EnvironmentOptions { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="engineService"></param>
        /// <param name="environmentOptions"></param>
        /// <param name="gameEngineRepository"></param>
        /// <param name="gameFactory"></param>
        /// <param name="leaderboardOptions"></param>
        public ProblemService(
            IStackIdentityResolver identityResolver,
            IProblemRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,
            IGameEngineService engineService,
            EnvironmentOptions environmentOptions,
            IGameFactory gameFactory,
            LeaderboardOptions leaderboardOptions
        )
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            EngineService = engineService ?? throw new ArgumentNullException(nameof(engineService));
            EnvironmentOptions = environmentOptions ?? throw new ArgumentNullException(nameof(environmentOptions));
        }

        /// <summary>
        /// get all by challenge
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Data.Problem, ProblemDetail>> GetAllByChallengeId(string challengeId, ProblemDataFilter dataFilter = null)
        {
            return await PagedResult<Data.Problem, ProblemDetail>(Repository.GetAllByChallengeLinkId(challengeId), dataFilter);
        }

        /// <summary>
        /// get problem by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProblemDetail> GetById(string id)
        {
            return Map<ProblemDetail>(await Repository.GetById(id));
        }

        /// <summary>
        /// create new problem
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProblemDetail> Create(ChallengeStart model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var game = GameFactory.GetGame();
            var board = game.Boards.FindByChallengeLinkId(model.ChallengeId);
            var challenge = board.GetChallengeById(model.ChallengeId);
            var teamBoard = Repository.DbContext.TeamBoards.Where(tb => tb.BoardId == board.Id && tb.TeamId == Identity.User.TeamId).FirstOrDefault();

            var problem = await Repository.DbContext.Problems
                .Include(p => p.Team)
                .ThenInclude(t => t.Users)
                .SingleOrDefaultAsync(p =>
                    p.TeamId == Identity.User.TeamId &&
                    p.ChallengeLinkId == model.ChallengeId);

            if (problem == null)
            {

                var entity = new Problem
                {
                    ChallengeLinkId = model.ChallengeId,
                    TeamId = Identity.User.TeamId,
                    BoardId = board.Id,
                    MaxSubmissions = board.MaxSubmissions,
                    Slug = challenge.Slug,
                    SharedId = board.AllowSharedWorkspaces // TODO: && challenge.HasSharedGamespace
                        ? teamBoard.SharedId
                        : null
                };

                await Repository.Add(entity);

                problem = await Repository.DbContext.Problems
                    .Include(p => p.Team)
                    .ThenInclude(t => t.Users)
                    .SingleOrDefaultAsync(p => p.Id == entity.Id);

                if (string.IsNullOrEmpty(problem.SharedId))
                {
                   problem.SharedId = problem.Id;
                   await Repository.Update(problem);
                }
            }

            var engineModel = BuildGameEngineProblem(game, board, challenge, problem, teamBoard, model.FlagIndex);
            await EngineService.Spawn(engineModel);

            return Map<ProblemDetail>(problem);
        }

        GameEngine.Models.Problem BuildGameEngineProblem(GameDetail game, BoardDetail board, ChallengeDetail challenge, Data.Problem problem, TeamBoard teamBoard, int? flagIndex)
        {
            var model = Map<GameEngine.Models.Problem>(problem);

            model.ChallengeLink = new GameEngine.Models.ChallengeLink
            {
                Id = challenge.Id,
                Slug = challenge.Slug
            };

            model.Settings = new GameEngine.Models.GameSettings
            {
                BoardId = board.Id,
                BoardName = board.Name,
                GameId = game.Id,
                GameName = game.Name,
                IsPractice = board.IsPractice,
                MaxSubmissions = board.MaxSubmissions
            };

            if (EnvironmentOptions.Mode == EnvironmentMode.Test && flagIndex.HasValue)
            {
                model.FlagIndex = flagIndex;
            }

            if (board.AllowSharedWorkspaces && !string.IsNullOrEmpty(problem.SharedId))
            {
                model.IsolationId = problem.SharedId;
            }
            else
            {
                model.IsolationId = problem.Id;
            }

            return model;
        }

        /// <summary>
        /// reset board
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ResetChallenges(TeamBoardReset model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var db = Repository.DbContext;

            var problems = await db.Problems
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)
                .ThenInclude(s => s.Tokens)
                .Where(p => p.BoardId == model.BoardId && p.TeamId == model.TeamId)
                .ToListAsync();

            var challengeIds = problems.Select(p => p.ChallengeLinkId).ToArray();
            var userIds = db.Users.Where(u => u.TeamId == model.TeamId).Select(u => u.Id).ToArray();

            RemoveSurveys(userIds, challengeIds);

            if (problems.Any())
            {
                foreach (var problem in problems)
                {
                    await RemoveProblemEntities(problem);
                }

                db.Problems.RemoveRange(problems);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// remove survey data related to challenge and user intersection
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="challengeIds"></param>
        void RemoveSurveys(string[] userIds, string[] challengeIds)
        {
            var remove = new List<Survey>();

            var surveys = Repository.DbContext.Surveys.Where(s => challengeIds.Contains(s.ChallengeId)).ToList();

            foreach (var userId in userIds)
            {
                var found = surveys.Where(s => s.UserId == userId).ToList();

                if (found.Any())
                {
                    remove.AddRange(found);
                }
            }

            if (remove.Any())
            {
                Repository.DbContext.Surveys.RemoveRange(remove);
            }
        }

        async Task RemoveProblemEntities(Problem problem)
        {
            if (problem == null)
                throw new EntityNotFoundException($"Could not remove releated objects from Problem. Problem not found.");

            var db = Repository.DbContext;

            if (problem.HasGamespace)
            {
                await DeleteGamespace(new GamespaceDelete { Id = problem.Id });
            }

            if (problem.Submissions.Any())
            {
                foreach (var submission in problem.Submissions)
                {
                    db.Tokens.RemoveRange(submission.Tokens);
                }
            }

            db.Tokens.RemoveRange(problem.Tokens);
        }

        /// <summary>
        /// reset challenge
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ResetChallenge(ChallengeReset model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            await RemoveProblem(model.TeamId, model.ChallengeLinkId);
        }

        /// <summary>
        /// if a challenge failed to start properly, allow restart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RestartChallenge(ChallengeRestart model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            await RemoveProblem(model.TeamId, model.ChallengeLinkId);
        }

        async Task RemoveProblem(string teamId, string challengeLinkId)
        {
            var problem = await Repository.DbContext.Problems
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)
                .ThenInclude(s => s.Tokens)
                .SingleOrDefaultAsync(p => p.ChallengeLinkId == challengeLinkId && p.TeamId == teamId);

            if (problem == null)
                throw new EntityNotFoundException($"Could not reset Challenge '{challengeLinkId}' for Team '{teamId}'. Problem not found.");

            await RemoveProblemEntities(problem);

            Repository.DbContext.Problems.Remove(problem);
            await Repository.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// get engine ticket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProblemConsoleSummary> GetTicket(ProblemConsole model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            return Map<ProblemConsoleSummary>(await EngineService.Ticket(model.VmId));
        }

        /// <summary>
        /// change console vm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ChangeVm(ProblemConsoleAction model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var action = Map<GameEngine.Models.VmAction>(model);
            await EngineService.ChangeVm(action);
        }

        /// <summary>
        /// start gamespace
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RestartGamespace(GamespaceRestart model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var game = GameFactory.GetGame();

            var problem = await Repository.DbContext.Problems
                .Include(p => p.Team)
                .ThenInclude(t => t.Users)
                .SingleOrDefaultAsync(p => p.Id == model.ProblemId);

            var board = game.Boards.FindByChallengeLinkId(problem.ChallengeLinkId);
            var challenge = board.GetChallengeById(problem.ChallengeLinkId);
            var teamBoard = Repository.DbContext.TeamBoards.Where(tb => tb.BoardId == board.Id && tb.TeamId == Identity.User.TeamId && tb.IsActive(board)).FirstOrDefault();

            var engineModel = BuildGameEngineProblem(game, board, challenge, problem, teamBoard, null);

            await EngineService.Spawn(engineModel);
        }

        /// <summary>
        /// delete gamespace
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task DeleteGamespace(GamespaceDelete model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var problem = await Repository.GetById(model.Id);

            await EngineService.Delete(problem.SharedId);

            foreach (var p in Repository.GetAll().Where(o => o.SharedId == problem.SharedId))
            {
                p.GamespaceReady = false;
                await Repository.Update(p);
            }
        }

        /// <summary>
        /// get game spaces
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Data.Problem, GamespaceDetail>> GetGamespaces(ProblemDataFilter dataFilter)
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
                throw new UnauthorizedAccessException("Action requires elevated permissions.");

            var q = Repository.GetAll()
                .Where(p => p.GamespaceReady)
                .OrderBy(p => p.Team.Name);

            return await PagedResult<Data.Problem, GamespaceDetail>(q, dataFilter, GetMappingOperationOptions());
        }

        /// <summary>
        /// expire gamespaces
        /// </summary>
        /// <returns></returns>
        public async Task ExpireGamespaces()
        {
            var problems = await DbContext.Problems
                .Include(p => p.Team)
                .ThenInclude(t => t.TeamBoards)
                .Where(p => p.GamespaceReady)
                .ToListAsync();

            foreach (var problem in problems)
            {
                var now = DateTime.UtcNow;
                var start = problem.Start;

                var game = GameFactory.GetGame();
                var board = game.Boards.FindByChallengeLinkId(problem.ChallengeLinkId);

                if (board == null)
                {
                    problem.GamespaceReady = false;
                    await DbContext.SaveChangesAsync();
                    continue;
                }

                int duration = board.MaxMinutes;

                var teamBoard = problem.Team.TeamBoards.SingleOrDefault(b => b.BoardId == board.Id);
                if (teamBoard != null)
                {
                    if (teamBoard.OverrideMaxMinutes.HasValue && teamBoard.OverrideMaxMinutes > 0)
                    {
                        duration = teamBoard.OverrideMaxMinutes.Value;
                    }

                    if (duration > 0)
                    {
                        start = teamBoard.Start;
                    }
                }

                if (duration == 0)
                {
                    duration = board.IsPractice ? 30 : 450;
                }

                // additional grace period
                duration += 30;

                if (start.AddMinutes(duration).CompareTo(now) < 0)
                {
                    await EngineService.Delete(problem.SharedId);
                    problem.GamespaceReady = false;
                }
            }

            await DbContext.SaveChangesAsync();

            var boards = GameFactory.GetGame().Boards
                .Where(b => b.AllowSharedWorkspaces).ToList();

            // loop through all boards that allow shared workspaces
            // get all questions for that board
            // get all problems for each team based on the question id
            // make sure each one is failed or success
            // determine if all problems for the board are done
            // if so, expire those with GamespaceReady

            List<string> teamIds = await DbContext.Teams.Select(t => t.Id).ToListAsync();

            foreach (var board in boards)
            {
                List<string> challengeLinkIds = new List<string>();
                int problemCounter = 0;

                // Get all questions for a board or map that are not disabled
                foreach (var category in board.Categories)
                {
                    challengeLinkIds.AddRange(category.Questions.Where(q => !q.IsDisabled && q.ChallengeLink != null).Select(q => q.ChallengeLink.Id));
                }

                foreach (var map in board.Maps)
                {
                    challengeLinkIds.AddRange(map.Coordinates.Where(q => !q.IsDisabled && q.ChallengeLink != null).Select(q => q.ChallengeLink.Id));
                }

                // get all problems for each team for this board and start testing
                foreach (string teamId in teamIds)
                {
                    var sharedProblems = DbContext.Problems.Where(p => p.BoardId == board.Id && p.TeamId == teamId && (p.Status.ToLower() == "success" || p.Status.ToLower() == "failure")).ToList();

                    if (sharedProblems.Any(p => p.GamespaceReady == true))
                    {
                        // if the finished problem count is less than question count we know the board is not finished
                        if (sharedProblems.Count >= challengeLinkIds.Count)
                        {
                            // make sure we have a problem instance for each question
                            foreach (string q in challengeLinkIds)
                            {
                                if (sharedProblems.Any(p => p.ChallengeLinkId == q))
                                {
                                    problemCounter++;
                                }
                            }

                            // if counter total == question total, you completed everything for the board
                            if (problemCounter == challengeLinkIds.Count)
                            {
                                List<string> sharedIds = sharedProblems.Where(p => p.GamespaceReady == true).Select(p => p.SharedId).Distinct().ToList();

                                foreach (string sharedId in sharedIds)
                                {
                                    await EngineService.Delete(sharedId);
                                }

                                foreach (Problem p in sharedProblems)
                                {
                                    if (p.GamespaceReady)
                                    {
                                        p.GamespaceReady = false;
                                        DbContext.Update(p);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
