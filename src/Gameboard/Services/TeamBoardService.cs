// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.ViewModels;
using GameEngine.Models;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// team board service
    /// </summary>
    public class TeamBoardService : Service<ITeamBoardRepository, TeamBoard>
    {
        IProblemRepository ProblemRepository { get; }
        ITeamRepository TeamRepository { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="teamRepository"></param>
        /// <param name="problemRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="gameFactory"></param>
        /// <param name="leaderboardOptions"></param>
        public TeamBoardService(IStackIdentityResolver identityResolver,
            ITeamBoardRepository repository,
            ITeamRepository teamRepository,
            IProblemRepository problemRepository,
            IMapper mapper,
            IValidationHandler validationHandler,            
            IGameFactory gameFactory,
            LeaderboardOptions leaderboardOptions
            )
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {            
            ProblemRepository = problemRepository;
            TeamRepository = teamRepository;
        }

        /// <summary>
        /// get teamboard by board and team
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<TeamBoardDetail> GetById(string boardId, string teamId)
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
            {
                throw new EntityPermissionException("User is not a moderator.");
            }

            return Map<TeamBoardDetail>(await Repository.GetById(boardId, teamId));
        }

        /// <summary>
        /// get teamboard by board id
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<TeamBoardDetail> GetScore(string boardId)
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
            {
                throw new EntityPermissionException("User is not a moderator or team member.");
            }

            var teamBoard = await Repository.GetAll().Where(tb => tb.TeamId == Identity.User.TeamId && tb.BoardId == boardId).SingleOrDefaultAsync();

            TeamBoardDetail teamBoardDetail = new TeamBoardDetail();

            if (teamBoard != null)
            {
                Mapper.Map(teamBoard, teamBoardDetail);

                // calculate the score for all problems
                var score = ProblemRepository.GetAll().Where(p => p.TeamId == Identity.User.TeamId && p.BoardId == boardId).Select(p => p.Score).Sum();
                teamBoardDetail.Score = score;
            }

            return teamBoardDetail;
        }

        /// <summary>
        /// override time remaining for team board
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> OverrideTimeRemaining(TeamBoardUpdate model)
        {
            if (!Identity.User.IsModerator)
            {
                throw new EntityPermissionException("User is not a moderator.");
            }

            var teamBoard = await Repository.GetById(model.BoardId, model.TeamId);

            if (teamBoard == null)
                throw new EntityNotFoundException("Team board was not found.");

            teamBoard.OverrideMaxMinutes = model.OverrideMaxMinutes;
            await Repository.Update(teamBoard);

            // update the team updated time so that leaderboard will be recalculated
            var team = await TeamRepository.GetById(teamBoard.TeamId);

            if (team != null)
            {
                team.Updated = DateTime.UtcNow;
                await TeamRepository.Update(team);
            }

            return Map<TeamDetail>(team);
        }

        /// <summary>
        /// get team board events
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<List<TeamBoardEventDetail>> GetTeamBoardEvents(string teamId)
        {
            if (Identity.User.TeamId != teamId && !(Identity.User.IsModerator || Identity.User.IsObserver))
                throw new EntityPermissionException("User does not have access to view team details.");

            var teamBoards = await DbContext.TeamBoards
                .Include(tb => tb.Team)
                .Where(p => p.TeamId == teamId)
                .ToListAsync();

            var problems = await DbContext.Problems
                .Include(p => p.Submissions)
                    .ThenInclude(s => s.Tokens)
                .Where(p => p.TeamId == teamId)
                .ToDictionaryAsync(p => p.ChallengeLinkId);

            var result = new List<TeamBoardEventDetail>();
            foreach (var tb in teamBoards)
            {
                var game = GameFactory.GetGame();
                var board = game.Boards.FirstOrDefault(b => b.Id == tb.BoardId);

                var challengeEvents = new List<ChallengeEventDetail>();
                if (board != null)
                {
                    if (board.BoardType == BoardType.Trivia)
                    {
                        AddCategories(problems, board, challengeEvents);
                    }

                    if (board.BoardType == BoardType.Map)
                    {
                        AddMaps(problems, board, challengeEvents);
                    }

                    var teamBoard = new TeamBoardEventDetail
                    {
                        Name = tb.Team.Name,
                        OverrideMaxMinutes = tb.OverrideMaxMinutes,
                        Score = tb.Score,
                        Start = tb.Start,
                        Board = board,
                        Challenges = challengeEvents
                    };

                    result.Add(teamBoard);
                }
            }

            return result.OrderBy(t => t.Board.Order).ToList();
        }

        /// <summary>
        /// map categories
        /// </summary>
        /// <param name="probs"></param>
        /// <param name="board"></param>
        /// <param name="challengeEventDetail"></param>
        static void AddCategories(Dictionary<string, Data.Problem> probs, BoardDetail board, List<ChallengeEventDetail> challengeEventDetail)
        {
            foreach (var category in board.Categories)
            {
                var questions = category.Questions
                    .Where(q => q.Challenge != null).ToList();

                foreach (var question in questions)
                {
                    var challenge = new ChallengeEventDetail
                    {
                        Name = category.Name,
                        Type = "Category",
                        Id = question.ChallengeLink.Id,
                        Points = question.Points,
                        Title = question.Challenge.Title,
                        Tags = question.Challenge.Tags
                    };

                    AddProblemEvents(probs, challenge);

                    challengeEventDetail.Add(challenge);
                }
            }
        }

        /// <summary>
        /// map maps
        /// </summary>
        /// <param name="probs"></param>
        /// <param name="board"></param>
        /// <param name="challengeEventDetail"></param>
        static void AddMaps(Dictionary<string, Data.Problem> probs, BoardDetail board, List<ChallengeEventDetail> challengeEventDetail)
        {
            foreach (var map in board.Maps)
            {
                var coordinates = map.Coordinates
                    .Where(c => c.Challenge != null).ToList();

                foreach (var coordinate in coordinates)
                {
                    var challenge = new ChallengeEventDetail
                    {
                        Name = map.Name,
                        Type = "Map",
                        Id = coordinate.ChallengeLink.Id,
                        Points = coordinate.Points,
                        Title = coordinate.Challenge.Title,
                        Tags = coordinate.Challenge.Tags
                    };

                    AddProblemEvents(probs, challenge);

                    challengeEventDetail.Add(challenge);
                }
            }
        }
        
        /// <summary>
        /// map problems
        /// </summary>
        /// <param name="probs"></param>
        /// <param name="challenge"></param>
        static void AddProblemEvents(Dictionary<string, Data.Problem> probs, ChallengeEventDetail challenge)
        {
            if (probs.ContainsKey(challenge.Id))
            {
                challenge.Events.Add(new ProblemEventDetail { Timestamp = probs[challenge.Id].Start });

                var submissions = probs[challenge.Id].Submissions
                    .OrderBy(s => s.Timestamp).ToList();

                foreach (var sub in submissions)
                {
                    var type = ProblemEventType.Default;

                    if (sub.Status == SubmissionStatus.Passed)
                    {
                        type = ProblemEventType.Success;
                    }

                    if (sub.Status == SubmissionStatus.Failed)
                    {
                        if (sub.Tokens.Any(t => t.Status == TokenStatusType.Correct))
                        {
                            type = ProblemEventType.Partial;
                        }
                        else
                        {
                            type = ProblemEventType.Failure;
                        }
                    }

                    challenge.Events.Add(new ProblemEventDetail
                    {
                        Timestamp = sub.Timestamp,
                        Type = type
                    });
                }
            }
        }
    }
}

