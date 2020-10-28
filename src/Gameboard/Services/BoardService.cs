// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    public class BoardService : Service
    {
        IGameEngineService EngineService { get; }
        GameboardDbContext DbContext { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="db"></param>
        /// <param name="engineService"></param>
        /// <param name="gameFactory"></param>
        /// <param name="leaderboardOptions"></param>
        public BoardService(IStackIdentityResolver identityResolver, IMapper mapper, IValidationHandler validationHandler,
            GameboardDbContext db, IGameEngineService engineService, IGameFactory gameFactory, LeaderboardOptions leaderboardOptions)
            : base(identityResolver, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            DbContext = db;
            EngineService = engineService;
        }

        /// <summary>
        /// get all boards from game engine cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BoardDetail> GetAll()
        {
            return GameFactory.GetGame().Boards;
        }

        /// <summary>
        /// get board with identity team details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BoardDetail> Get(BoardRequest model)
        {
            return await Get(model, Identity.User.TeamId);
        }

        /// <summary>
        /// get board with team details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<BoardDetail> Get(BoardRequest model, string teamId)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var game = GameFactory.GetGame();

            var board = game.Boards.FirstOrDefault(b => b.Id == model.Id);
            var challengeModels = board.GetChallengeModels();

            DateTime? teamBoardStart = null;

            if (!string.IsNullOrWhiteSpace(teamId))
            {
                if (teamId != Identity.User.TeamId && !(Identity.User.IsModerator || Identity.User.IsObserver))
                    throw new EntityPermissionException("Action requires elevated permissions.");

                var team = await DbContext.Teams
                    .Include(t => t.TeamBoards)
                    .SingleOrDefaultAsync(t => t.Id == teamId);
                
                int? overrideMaxMinutes = null;

                if (team != null)
                {
                    var teamBoard = team.TeamBoards.FirstOrDefault(tb => tb.BoardId == board.Id);

                    if (teamBoard != null)
                    {                        
                        overrideMaxMinutes = teamBoard.OverrideMaxMinutes;
                        teamBoardStart = teamBoard.Start;
                    }
                }

                board.IsActive = board.IsActive(teamBoardStart, overrideMaxMinutes);

                var boardEnd = board.Ends(teamBoardStart, overrideMaxMinutes);

                if (teamBoardStart.HasValue)
                {
                    var challengeProblems = await DbContext.Problems
                        .Include(p => p.Submissions)
                        .Where(p => p.TeamId == teamId && p.BoardId == model.Id)
                        .ToDictionaryAsync(p => p.ChallengeLinkId);
                    
                    foreach (var challengeModel in challengeModels)
                    {
                        if (challengeProblems.ContainsKey(challengeModel.ChallengeLink.Id))
                        {
                            var problem = challengeProblems[challengeModel.ChallengeLink.Id];
                            var start = problem.Start;
                            var now = DateTime.UtcNow;
                            DateTime? end = null;

                            var status = problem.Status ?? "None";

                            switch (status.ToLower())
                            {
                                case "success":
                                    end = problem.End ?? now;
                                    break;
                                case "failure":
                                    end = problem.Submissions.Any() ? problem.Submissions.Last().Timestamp : now;
                                    break;
                                case "registered":
                                case "ready":
                                case "error":
                                default:
                                    break;
                            }

                            end = end ?? boardEnd ?? now;

                            var totalMinutes = (end.Value - start).TotalMinutes;

                            challengeModel.Challenge.ProblemId = problem.Id;
                            challengeModel.Challenge.ProblemScore = problem.Score;
                            challengeModel.Challenge.TotalMinutes = Math.Round(totalMinutes, 1);
                            challengeModel.Challenge.ProblemStatus = problem.Status;
                            challengeModel.Challenge.GamespaceReady = problem.GamespaceReady;
                            challengeModel.Challenge.HasGamespace = problem.HasGamespace;
                        }                        
                    }
                }
            }

            if (!teamBoardStart.HasValue && !board.IsPreviewAllowed)
            {
                foreach (var cm in challengeModels)
                {
                    cm.Challenge = new ChallengeDetail();
                }
            }

            return board;
        }        

        /// <summary>
        /// get team board by board id and identity team
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<TeamBoardDetail> GetTeamBoard(string boardId)
        {
            var teamBoard = await DbContext.TeamBoards
                .Include(tb => tb.Team)
                .SingleOrDefaultAsync(tb => tb.BoardId == boardId && tb.TeamId == Identity.User.TeamId);

            if (teamBoard == null)
                return null;

            return Map<TeamBoardDetail>(teamBoard);
        }

        public async Task<TeamBoardStatus> GetSessionForecast(string boardId)
        {
            return new TeamBoardStatus
            {
                TeamBoard = await GetTeamBoard(boardId),
                Forecast = await EngineService.GetForecast()
            };
        }

        /// <summary>
        /// get challenge surveys by board id
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<ChallengeSurveyReport> GetChallengeSurveyReport(string boardId)
        {
            var game = GameFactory.GetGame();

            var board = game.Boards.SingleOrDefault(b => b.Id == boardId);

            var challenges = board.GetChallenges();

            var challengeIds = challenges.Select(c => c.Id).ToArray();

            var surveys = await DbContext.Surveys
                .Where(s => challengeIds.Contains(s.ChallengeId))
                .ToListAsync();

            var userIds = surveys.Select(p => p.UserId).ToArray();

            var users = await DbContext.Users
                .Include(u => u.Team)
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var result = new ChallengeSurveyReport
            {
                BoardId = board.Id,
                BoardName = board.Name
            };

            foreach (var s in surveys)
            {
                var responses = JsonConvert.DeserializeObject<List<SurveyResponse>>(s.Data);

                foreach (var response in responses)
                {
                    var item = new ChallengSurveyItem
                    {
                        ChallengeId = s.ChallengeId,
                        Date = s.Created,                        
                        ChallengeTitle = challenges.SingleOrDefault(c => c.Id == s.ChallengeId)?.Title ?? "N/A",                        
                        Question = response.Text,
                        Answer = response.Value
                    };

                    result.Items.Add(item);
                }
            }

            result.Items = result.Items
                .OrderBy(r => r.Date)
                .ThenBy(r => r.ChallengeTitle)
                .ThenBy(r => r.Question)
                .ToList();

            return result;
        }

        /// <summary>
        /// get all board completion reports
        /// </summary>
        /// <returns></returns>
        public async Task<List<BoardCompletionReport>> GetBoardCompletionReports()
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
                throw new EntityPermissionException("User is not a moderator.");

            var boardReports = new List<BoardCompletionReport>();

            var game = GameFactory.GetGame();
            var boards = game.Boards
                .Where(b => !b.IsPractice).ToList();

            foreach (var board in boards)
            {
                var boardReport = await GetBoardCompletionReport(board);

                if (boardReport.Items.Any())
                {
                    boardReports.Add(boardReport);
                }
            }

            return boardReports;
        }

        /// <summary>
        /// get board completion report by board id
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<BoardCompletionReport> GetBoardCompletionReport(string boardId)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == boardId);

            return await GetBoardCompletionReport(board);
        }

        /// <summary>
        /// export board completion report
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<List<BoardCompletionReportExport>> ExportBoardCompletionReport(string boardId)
        {
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == boardId);

            var report = await GetBoardCompletionReport(board);

            var result = new List<BoardCompletionReportExport>();

            foreach (var item in report.Items)
            {
                foreach (var challenge in item.Challenges)
                {
                    var export = new BoardCompletionReportExport
                    {
                        BoardId = report.Id,
                        BoardName = report.Name,
                        BoardType = report.BoardType,
                        ContainerId = item.Id,
                        ContainerName = item.Name,                        
                        ChallengeId = challenge.Id,
                        ChallengeTitle = challenge.Title,
                        AverageMilliseconds = challenge.AverageMilliseconds,
                        Points = challenge.Points,
                        Total = challenge.Total,
                        FailureCount = challenge.Failure.Count,
                        FailureRatio = challenge.Failure.Ratio,
                        PartialCount = challenge.Partial.Count,
                        PartialRatio = challenge.Partial.Ratio,                        
                        SuccessCount = challenge.Success.Count,
                        SuccessRatio = challenge.Success.Ratio                        
                    };

                    result.Add(export);
                }
            }

            return result;
        }

        /// <summary>
        /// get board completion report by board detail
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        async Task<BoardCompletionReport> GetBoardCompletionReport(BoardDetail board)
        {
            if (board == null)
                return null;

            var boardReport = new BoardCompletionReport
            {
                Id = board.Id,
                Name = board.Name,
                BoardType = board.BoardType
            };

            var problems = await DbContext.Problems
                .Where(p => p.BoardId == board.Id && p.Submissions.Any())
                .Select(p => new 
                { 
                    p.Score,
                    p.Start,
                    p.End,
                    p.Status,
                    p.ChallengeLinkId
                })
                .ToListAsync();

            if (problems.Any())
            {
                var statistics = new List<BoardCompletionQueryCounts>();

                var query = problems.GroupBy(p => p.ChallengeLinkId);

                foreach (var grouping in query)
                {
                    var challengeLinkId = grouping.Key;

                    var challenge = board.GetChallengeById(challengeLinkId);

                    var total = grouping.Count();

                    var success = grouping.Count(p => p.Status == "Success");
                    
                    var averageMilliseconds = 0.0;

                    if (success > 0)
                    {
                        var totalMinutes = grouping
                            .Where(p => p.End.HasValue && p.Status == "Success")
                            .Sum(p => (p.End.Value - p.Start).TotalMilliseconds);

                        averageMilliseconds = totalMinutes / success;
                    }

                    var count = new BoardCompletionQueryCounts
                    {
                        ChallengeLinkId = challengeLinkId,
                        Total = total,
                        Success = grouping.Count(p => p.Score == challenge.Points),
                        Partial = grouping.Count(p => p.Score > 0 && p.Score < challenge.Points),
                        Failure = grouping.Count(p => p.Score == 0),
                        AverageMilliseconds = averageMilliseconds
                    };

                    statistics.Add(count);
                }

                if (board.BoardType == BoardType.Trivia)
                {
                    GetTriviaBoardCompletionReport(board, boardReport, statistics);
                }

                if (board.BoardType == BoardType.Map)
                {
                    GetMapBoardCompletionReport(board, boardReport, statistics);
                }
            }

            return boardReport;
        }

        void GetMapBoardCompletionReport(BoardDetail board, BoardCompletionReport boardReport, IEnumerable<BoardCompletionQueryCounts> statistics)
        {
            var maps = board.Maps.OrderBy(c => c.Order);

            foreach (var map in maps)
            {
                var item = new BoardCompletionReportItem
                {
                    Id = map.Id,
                    Name = map.Name
                };

                var coordinates = map.Coordinates.Where(c => c.Challenge != null).ToList();

                foreach (var coordinate in coordinates)
                {
                    var statistic = statistics
                        .SingleOrDefault(ps => ps.ChallengeLinkId == coordinate.ChallengeLink.Id);

                    item.Challenges.Add(GetBoardCompleteReportChallenge(coordinate, statistic));
                }

                if (item.Challenges.Any())
                {
                    boardReport.Items.Add(item);
                }
            }
        }

        void GetTriviaBoardCompletionReport(BoardDetail board, BoardCompletionReport boardReport, IEnumerable<BoardCompletionQueryCounts> statistics)
        {
            var categories = board.Categories.OrderBy(c => c.Order);

            foreach (var category in board.Categories)
            {
                var item = new BoardCompletionReportItem
                {
                    Id = category.Id,
                    Name = category.Name
                };

                var questions = category.Questions.Where(q => q.Challenge != null).ToList();

                foreach (var question in questions)
                {
                    var statistic = statistics
                        .SingleOrDefault(ps => ps.ChallengeLinkId == question.ChallengeLink.Id);

                    item.Challenges.Add(GetBoardCompleteReportChallenge(question, statistic));
                }

                if (item.Challenges.Any())
                {
                    boardReport.Items.Add(item);
                }
            }
        }

        class BoardCompletionQueryCounts
        {
            public string ChallengeLinkId { get; set; }
            public int Success { get; set; }
            public int Partial { get; set; }
            public int Failure { get; set; }
            public int Total { get; set; }
            public double AverageMilliseconds { get; set; }
        }

        /// <summary>
        /// get board completion report challenge by challenge model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="statistic"></param>
        /// <returns></returns>
        BoardCompletionReportChallenge GetBoardCompleteReportChallenge(IChallengeModel model, BoardCompletionQueryCounts statistic)
        {
            double total = 0;
            double successCount = 0;
            double partialCount = 0;
            double failureCount = 0;
            double averageMilliseconds = 0.0;

            if (statistic != null)
            {
                total = statistic.Total;
                successCount = statistic.Success;
                partialCount = statistic.Partial;
                failureCount = statistic.Failure;
                averageMilliseconds = statistic.AverageMilliseconds;
            }

            var success = new BoardCompletionReportChallengeStat
            {
                Count = (int)successCount,
                Ratio = total > 0 ? (successCount / total) * 100.0 : 0
            };

            var failure = new BoardCompletionReportChallengeStat
            {
                Count = (int)failureCount,
                Ratio = total > 0 ? (failureCount / total) * 100.0 : 0
            };

            var partial = new BoardCompletionReportChallengeStat
            {
                Count = (int)partialCount,
                Ratio = total > 0 ? (partialCount / total) * 100.0 : 0
            };

            return new BoardCompletionReportChallenge
            {
                Id = model.ChallengeLink.Id,
                Points = model.Points,
                Title = model.Challenge.Title,
                Success = success,
                Failure = failure,
                Partial = partial,
                AverageMilliseconds = averageMilliseconds,
                Total = (int)total
            };
        }
    }
}

