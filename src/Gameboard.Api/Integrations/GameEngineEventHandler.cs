// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using GameEngine.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Integrations
{
    /// <summary>
    /// callback handler for game engine
    /// </summary>
    public class GameEngineEventHandler : IGameEngineEventHandler
    {
        ILogger Logger { get; }
        IMapper Mapper { get; }
        IProblemRepository ProblemRepository { get; }
        ISubmissionRepository SubmissionRepository { get; }
        IHubContext<GameboardHub, IGameboardEvent> Hub { get; }
        TeamService TeamService { get; }        
        IGameFactory GameFactory { get; }
        IGameEngineService EngineService { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="problemRepository"></param>
        /// <param name="submissionRepository"></param>
        /// <param name="teamService"></param>
        /// <param name="mapper"></param>
        /// <param name="hub"></param>
        /// <param name="gameFactory"></param>        
        public GameEngineEventHandler(
            ILogger<GameEngineEventHandler> logger,
            IProblemRepository problemRepository,
            ISubmissionRepository submissionRepository,
            TeamService teamService,
            IMapper mapper,
            IHubContext<GameboardHub, IGameboardEvent> hub,
            IGameFactory gameFactory,
            IGameEngineService engineService
        )
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            ProblemRepository = problemRepository ?? throw new ArgumentNullException(nameof(problemRepository));
            SubmissionRepository = submissionRepository ?? throw new ArgumentNullException(nameof(submissionRepository));
            TeamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            Hub = hub ?? throw new ArgumentNullException(nameof(hub));
            GameFactory = gameFactory ?? throw new ArgumentNullException(nameof(gameFactory));
            EngineService = engineService ?? throw new ArgumentNullException(nameof(engineService));
        }

        /// <summary>
        /// call the game factory to refresh
        /// </summary>
        /// <returns></returns>
        public async Task Reload()
        {
            await GameFactory.Refresh();
        }

        /// <summary>
        /// update problem and notify
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task Update(ProblemState state)
        {
            Logger.LogDebug($"Handling engine event [updated] for {state.Id} - received");

            var problem = ProblemRepository.GetById(state.Id).Result;
            if (problem == null)
                return;

            var game = GameFactory.GetGame();

            var challenge = game.FindChallengeByChallengeLinkId(problem.ChallengeLinkId);
            if (challenge == null)
                return;

            problem.GamespaceReady = state.GamespaceReady;
            problem.HasGamespace = state.HasGamespace;
            problem.Status = state.Status.ToString();

            problem.Score = (challenge.Points / 100.0) * state.Percent;

            problem.Text = state.Text;
            problem.GamespaceText = state.GamespaceText;

            if (state.Start > problem.Start)
                problem.Start = state.Start;

            problem.End = state.End;

            await MapProblemTokens(problem.Id, state.Tokens, problem);

            var board = game.Boards.FindByChallengeLinkId(problem.ChallengeLinkId);

            if (problem.Status.ToLower() == "success" || problem.Status.ToLower() == "failure")
            {
                if (!board.AllowSharedWorkspaces)
                {
                    await EngineService.Delete(problem.SharedId);
                    problem.GamespaceReady = false;
                }
            }

            ProblemRepository.Update(problem).Wait();
            Logger.LogDebug($"Handling engine event [updated] for {state.Id} - saved");

            var model = Mapper.Map<ProblemDetail>(problem);
            model.EstimatedReadySeconds = state.EstimatedReadySeconds;

            Hub.Clients.Group(problem.TeamId).ProblemUpdated(model).Wait();

            if (model.Score > 0)
            {            
                TeamService.CalculateScore(board.Id, problem.TeamId);
            }

            var team = TeamService.GetById(problem.TeamId).Result;
            Hub.Clients.Group(problem.TeamId).TeamUpdated(team).Wait();

            Logger.LogDebug($"Handling engine event [updated] for {state.Id} - broadcasted");
        }        

        /// <summary>
        /// handle graded flag and notify
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task Update(GradedSubmission flag)
        {
            Logger.LogDebug($"Handling engine event [graded] for {flag.ProblemId} - received");

            var entity = SubmissionRepository.GetById(flag.SubmissionId).Result;

            if (entity != null)
            {
                entity.Status = flag.Status;
                entity.Timestamp = flag.Timestamp;
                
                await MapSubmissionTokens(flag.SubmissionId, flag.Tokens, entity);

                var team = TeamService.GetById(entity.Problem.TeamId).Result;
                
                Hub.Clients.Group(entity.Problem.TeamId).TeamUpdated(team).Wait();

                Logger.LogDebug($"Handling engine event [graded] for {flag.ProblemId} - saved");
                Update(flag.State).Wait();
            }
        }

        /// <summary>
        /// map the problem state tokens to the tokens collection
        /// </summary>        
        async Task MapProblemTokens(string problemId, List<GameEngine.Abstractions.Models.Token> tokens, Data.Problem parent)
        {
            var db = SubmissionRepository.DbContext;

            var ordered = tokens.OrderBy(x => x.Index);            

            foreach (var t in ordered)
            {
                var token = parent.Tokens.SingleOrDefault(x => x.Index == t.Index);

                if (token == null) 
                {
                    token = new Token();
                    parent.Tokens.Add(token);
                }

                token.Index = t.Index;
                token.Label = t.Label;
                token.Percent = t.Percent;
                token.Status = t.Status;
                token.Timestamp = t.Timestamp;
                token.Value = t.Value;
                token.ProblemId = problemId;
            };

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// map submission tokens to entity tokens
        /// </summary>
        /// <param name="submissionId"></param>
        /// <param name="tokens"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        async Task MapSubmissionTokens(string submissionId, List<GameEngine.Abstractions.Models.Token> tokens, Submission parent)
        {
            var ordered = tokens.OrderBy(x => x.Index);

            parent.Tokens.Clear();

            foreach (var t in ordered)
            {
                parent.Tokens.Add(new Token
                {
                    Index = t.Index,
                    Label = t.Label,
                    Percent = t.Percent,
                    Status = t.Status,
                    Timestamp = t.Timestamp,
                    Value = t.Value,
                    SubmissionId = submissionId
                });
            };

            await SubmissionRepository.DbContext.SaveChangesAsync();
        }
    }
}

