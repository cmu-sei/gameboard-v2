// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using GameEngine.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
    /// submission service
    /// </summary>
    public class SubmissionService : Service<ISubmissionRepository, Submission>
    {
        public SubmissionService(
            IStackIdentityResolver identityResolver,
            ISubmissionRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,
            IGameEngineService engineService,
            IGameFactory gameFactory,
            LeaderboardOptions leaderboardOptions
        )
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            EngineService = engineService;
        }

        IGameEngineService EngineService { get; }

        /// <summary>
        /// get all by challenge
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Submission, SubmissionDetail>> GetAll(string challengeId, SubmissionDataFilter dataFilter = null)
        {
            var teamId = Identity.User.TeamId;
            return await PagedResult<Submission, SubmissionDetail>(Repository.GetAll(challengeId, teamId), dataFilter);
        }

        /// <summary>
        /// get submission by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SubmissionDetail> GetById(string id)
        {
            return Map<SubmissionDetail>(await Repository.GetById(id));
        }

        /// <summary>
        /// calculate submission count
        /// </summary>
        /// <param name="board"></param>
        /// <param name="challenge"></param>
        /// <param name="submissions"></param>
        /// <returns></returns>
        public static int CalculateSubmissionCount(BoardDetail board, ChallengeDetail challenge, IEnumerable<Submission> submissions)
        {
            if (board.MaxSubmissions <= 0 || !submissions.Any())
                return 0;

            if (!challenge.IsMultiStage)
                return submissions.Count(x => x.Status != SubmissionStatus.Submitted);

            var currentMultiStageTokenIndex = 0;
            var multiStageTokenIncorrectCounts = new int[challenge.TokenCount];

            var orderedSubmissions = submissions.OrderBy(s => s.Timestamp);

            foreach (var s in orderedSubmissions)
            {
                var orderedTokens = s.Tokens.OrderBy(t => t.Index).ToList();

                orderedTokens.ForEach(t =>
                {
                    if (t.Status == TokenStatusType.Incorrect)
                        multiStageTokenIncorrectCounts[t.Index.Value]++;

                    if (t.Status == TokenStatusType.Correct)
                        currentMultiStageTokenIndex = t.Index.Value + 1;
                });
            }

            if (currentMultiStageTokenIndex >= multiStageTokenIncorrectCounts.Length)
                return board.MaxSubmissions;

            return multiStageTokenIncorrectCounts[currentMultiStageTokenIndex];
        }

        /// <summary>
        /// create new submission
        /// </summary>
        /// <param name="model"></param>
        /// <param name="broadcastAction"></param>
        /// <returns></returns>
        public async Task<SubmissionDetail> Create(SubmissionCreate model, Action<ProblemDetail, TeamDetail> broadcastAction = null)
        {
            await ValidationHandler.ValidateRulesFor(model);
            
            var submission = Map<Submission>(model);
            submission.UserId = Identity.Id;

            var saved = await Repository.Add(submission);

            var game = GameFactory.GetGame();
            var challenge = game.Boards.GetChallenges().FirstOrDefault(c => c.Id == submission.Problem.ChallengeLinkId);
            var board = game.Boards.FirstOrDefault(b => b.Id == submission.Problem.BoardId);

            var submissions = DbContext.Submissions.AsNoTracking()
                .Include(s => s.Tokens)
                .Where(s => s.ProblemId == model.ProblemId)
                .OrderBy(s => s.Timestamp)
                .ToList();            

            var problemFlag = Map<ProblemFlag>(saved);

            // count is equal to the current number of incorrect submissions plus the current
            problemFlag.Count = CalculateSubmissionCount(board, challenge, submissions) + 1;

            await EngineService.Grade(problemFlag);

            return await GetById(saved.Id);
        }
    }
}

