// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Cache;
using Gameboard.Data;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    public class LeaderboardService : Service
    {
        IGameboardCache<Leaderboard> LeaderboardCache { get; }
        
        GameboardDbContext DbContext { get; }

        public LeaderboardService(
            IStackIdentityResolver identityResolver,
            GameboardDbContext db,
            IGameboardCache<Leaderboard> leaderboardCache,
            IGameFactory gameFactory,
            LeaderboardOptions leaderboardOptions,
            IMapper mapper,
            IValidationHandler validationHandler)
            : base(identityResolver, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
            LeaderboardCache = leaderboardCache ?? throw new ArgumentNullException(nameof(leaderboardCache));
        }

        /// <summary>
        /// get leaderboard from cache
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        Leaderboard GetById(string boardId)
        {
            return LeaderboardCache.Get($"{LeaderboardOptions.CacheKey}-{boardId}")
                 ?? new Leaderboard { BoardId = boardId };
        }

        /// <summary>
        /// get leaderboard for board
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<Leaderboard> Get(string boardId, LeaderboardDataFilter dataFilter = null)
        {
            return await ApplyDataFilter(GetById(boardId), dataFilter);
        }

        /// <summary>
        /// export leaderboard
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public List<LeaderboardExport> Export(string boardId)
        {
            var leaderboard = GetById(boardId);

            var list = new List<LeaderboardExport>();

            foreach (var item in leaderboard.Results)
            {
                var export = new LeaderboardExport
                {
                    Id = item.Id,
                    AnonymizedName = item.AnonymizedName,
                    Badges = string.Join(" ", item.Badges),
                    Duration = item.Duration,                    
                    IsDisabled = item.IsDisabled,
                    MaxMinutes = item.MaxMinutes,
                    Name = item.Name,
                    Number = item.Number,
                    OrganizationalUnitLogoUrl = item.OrganizationalUnitLogoUrl,
                    OrganizationLogoUrl = item.OrganizationLogoUrl,
                    OrganizationName = item.OrganizationName,                    
                    Rank = item.Rank,
                    Score = item.Score,
                    Start = item.Start,
                    FailureCount = item.Counts.Failure,
                    PartialCount = item.Counts.Partial,
                    SuccessCount = item.Counts.Success,
                    TotalCount = item.Counts.Total
                };

                list.Add(export);
            }

            return list;
        }

        async Task<Leaderboard> ApplyDataFilter(Leaderboard leaderboard, LeaderboardDataFilter dataFilter)
        {
            var teamBoards = await DbContext.TeamBoards.Where(t => t.BoardId == leaderboard.BoardId).ToListAsync();
            var game = GameFactory.GetGame();
            var board = game.Boards.SingleOrDefault(b => b.Id == leaderboard.BoardId);

            var filtered = new Leaderboard()
            {
                BoardId = leaderboard.BoardId,
                DataFilter = dataFilter,
                Timestamp = leaderboard.Timestamp,
                Total = leaderboard.Results.Count,
                TotalActive = teamBoards.Count(t => t.IsActive(board)),
                TotalTeams = teamBoards.Count
            };

            var results = leaderboard.Results.Clone();

            // anonymize before processing data filter to avoid actual name search on anonymized teams
            results.ForEach(s =>
            {
                if (s.IsAnonymizeTeam(LeaderboardOptions, Identity))
                {
                    s.Name = s.AnonymizedName;
                }
            });

            var query = results.AsQueryable();

            if (dataFilter != null)
            {
                query = dataFilter.SearchQuery(query);
                query = dataFilter.FilterQuery(query, Identity);
                query = dataFilter.SortQuery(query);

                if (dataFilter.Skip > 0)
                    query = query.Skip(dataFilter.Skip);

                if (dataFilter.Take > 0)
                    query = query.Take(dataFilter.Take);
            }

            filtered.Results = query.ToList();

            return filtered;
        }

        /// <summary>
        /// get team score
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public LeaderboardScore GetTeamScore(string boardId, string teamId)
        {
            var game = GameFactory.GetGame();

            var leaderboard = GetById(boardId);

            var score = leaderboard.Results.SingleOrDefault(s => s.Id == teamId)
                ?? new LeaderboardScore { Id = teamId };

            if (LeaderboardOptions.Anonymize)
                score.Name = TeamExtensions.AnonymizeTeamName(score.OrganizationName, game.AnonymizeTag, score.Number);

            return score;
        }

        DateTime? GetLatest(IEnumerable<Submission> submissions, IEnumerable<Team> teamUpdates, string id)
        {
            var latestSubmission = submissions
                .FirstOrDefault(s => s.Problem.BoardId == id && s.Problem.Score > 0);

            var latestTeamUpdate = teamUpdates
                .FirstOrDefault(t => t.TeamBoards.Any(tb => tb.BoardId == id));

            DateTime? latest = null;

            if (latestSubmission != null)
            {
                latest = latestSubmission.Timestamp;
            }

            if (latestTeamUpdate != null && latest < latestTeamUpdate.Updated)
            {
                latest = latestTeamUpdate.Updated;
            }

            return latest;
        }

        public List<Leaderboard> Calculate()
        {
            var leaderboards = new List<Leaderboard>();

            var game = GameFactory.GetGame();

            if (game == null)
                return leaderboards;

            var boards = game.Boards.ToArray();

            var boardIds = boards.Select(b => b.Id).ToArray();

            var submissions = DbContext.Submissions
                .Include(s => s.Problem)
                .Where(s => boardIds.Contains(s.Problem.BoardId) && s.Problem.Score > 0)
                .OrderByDescending(s => s.Timestamp)
                .ToList();

            var teamUpdates = DbContext.Teams
                .Include(t => t.TeamBoards)
                .Where(t =>
                    t.Updated.HasValue &&
                    t.TeamBoards.Any(tb => boardIds.Contains(tb.BoardId)))
                .OrderByDescending(t => t.Updated)
                .ToList();

            foreach (var board in boards)
            {
                string id = board.Id;

                var key = $"{LeaderboardOptions.CacheKey}-{id}";

                var leaderboard = LeaderboardCache.Get(key)
                    ?? new Leaderboard { BoardId = id };

                var teamProblems = new List<IGrouping<Team, Problem>>();

                var currentTimestamp = leaderboard.Timestamp;

                var latest = GetLatest(submissions, teamUpdates, id);

                if (currentTimestamp < latest)
                {
                    leaderboard.Timestamp = latest.Value;

                    teamProblems = DbContext.Problems
                        .Include(p => p.Submissions)
                            .ThenInclude(s => s.Tokens)
                        .Include(p => p.Team)
                            .ThenInclude(tp => tp.TeamBoards)
                        .Where(p => p.BoardId == id)
                        .GroupBy(p => p.Team)
                        .ToList();

                    var scores = new List<LeaderboardScore>();

                    foreach (var teamProblem in teamProblems)
                    {
                        var team = teamProblem.Key;
                        var teamBoard = teamProblem.Key.TeamBoards.FirstOrDefault(tb => tb.TeamId == team.Id && tb.BoardId == id);
                        var problems = teamProblem.ToList();

                        var challengeProblems = problems.Select(p => new
                        {
                            Challenge = game.FindChallengeByChallengeLinkId(p.ChallengeLinkId),
                            Problem = p
                        });

                        // get all problems with a max score
                        var success = challengeProblems.Count(cp => cp.Challenge != null && 
                            cp.Challenge.Points == cp.Problem.Score);

                        // get all problems with no score and submissions
                        var failure = challengeProblems.Count(cp => cp.Challenge != null && 
                            cp.Problem.Score == 0.0 && cp.Problem.Submissions.Any());

                        // get all problems with a positive score
                        var partial = challengeProblems.Count(cp => cp.Challenge != null && 
                            cp.Problem.Score > 0.0 && cp.Problem.Score < cp.Challenge.Points);

                        // collection of problems and most recent successful submission
                        var scored = new Dictionary<Problem, Submission>();

                        var points = problems.Where(p => p.Score > 0.0);

                        foreach (var problem in points)
                        {
                            var ordered = problem.Submissions.OrderBy(s => s.Timestamp).ToList();

                            foreach (var submission in ordered)
                            {
                                Submission found = null;

                                if (scored.ContainsKey(problem))
                                {
                                    // submission already added for problem
                                    found = scored[problem];
                                }

                                if (found == null)
                                {
                                    // problem not found, add new submission
                                    scored.Add(problem, submission);                                    
                                }
                                else
                                {
                                    var last = found.Tokens
                                        .Where(t => t.Status == TokenStatusType.Correct)
                                        .Sum(t => t.Percent);

                                    var percent = submission.Tokens
                                        .Where(t => t.Status == TokenStatusType.Correct)
                                        .Sum(t => t.Percent);

                                    // if new submission percent total is > previous percent total 
                                    if (percent > last)
                                    {
                                        scored[problem] = submission;
                                    }
                                }
                            }
                        }

                        var duration = scored.Any()
                            ? scored.Sum(s => (s.Value.Timestamp - s.Key.Start).TotalMilliseconds)
                            : 0;

                        var score = new LeaderboardScore
                        {
                            Id = team.Id,
                            Name = team.Name,
                            Number = team.Number,
                            Badges = team.Badges.ToBadgeArray(),
                            IsDisabled = team.IsDisabled,
                            Score = problems.Sum(p => p.Score),                            
                            Counts = new LeaderboardScoreCounts
                            { 
                                Success = success,
                                Failure = failure,
                                Partial = partial,
                                Total = problems.Count()
                            },
                            Duration = duration,
                            OrganizationLogoUrl = team.OrganizationLogoUrl,
                            OrganizationalUnitLogoUrl = team.OrganizationalUnitLogoUrl,
                            OrganizationName = team.OrganizationName,
                            Start = teamBoard?.Start,
                            MaxMinutes = teamBoard?.OverrideMaxMinutes ?? board.MaxMinutes
                        };

                        score.AnonymizedName = score.AnonymizeTeamName(game);

                        //TODO: Make this a board property
                        // Board.MinimumLeaderboardScore
                        if (score.Score > 0)
                        {
                            scores.Add(score);
                        }
                    }

                    leaderboard.Results = scores
                        .OrderByDescending(s => s.Score)
                        .ThenBy(s => s.Duration)
                        .ThenBy(s => s.Counts.Success)
                        .ThenBy(s => s.Name)
                        .ToList();

                    int rank = 1;
                    foreach (var score in leaderboard.Results)
                        score.Rank = rank++;

                    LeaderboardCache.Set(key, leaderboard);

                    var summary = ApplyDataFilter(leaderboard, new LeaderboardDataFilter { Skip = 0, Take = 10, Sort = "rank" }).Result;
                    leaderboards.Add(summary);
                }
            }

            return leaderboards;
        }
    }
}

