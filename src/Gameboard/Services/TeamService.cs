// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Cache;
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
    /// team service
    /// </summary>
    public class TeamService : Service<ITeamRepository, Team>
    {
        IGameboardCache<User> UserCache { get; }
        IGameEngineService EngineService { get; }
        IProblemRepository ProblemRepository { get; }

        /// <summary>
        /// create an instance of team service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="problemRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="userCache"></param>
        /// <param name="leaderboardOptions"></param>
        /// <param name="engineService"></param>
        /// <param name="gameFactory"></param>        
        public TeamService(
            IStackIdentityResolver identityResolver,
            ITeamRepository repository,
            IProblemRepository problemRepository,
            IMapper mapper,
            IValidationHandler validationHandler,
            IGameboardCache<User> userCache,
            LeaderboardOptions leaderboardOptions,
            IGameEngineService engineService,
            IGameFactory gameFactory)
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            UserCache = userCache;
            EngineService = engineService;
            ProblemRepository = problemRepository;
        }

        /// <summary>
        /// clear all cached users related to team
        /// </summary>
        /// <param name="team"></param>
        public void RemoveUsersFromCache(Team team)
        {
            if (team == null)
                return;

            foreach (var user in team.Users)
                UserCache.Remove(user.Id);
        }

        /// <summary>
        /// get all teams
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Team, TeamDetail>> GetAll(TeamDataFilter dataFilter = null)
        {
            return await PagedResult<Team, TeamDetail>(Repository.GetAll(), dataFilter);
        }

        /// <summary>
        /// export team list
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TeamExport>> ExportTeams()
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var teams = await Repository.GetAll()
                .OrderBy(t => t.Name)
                .ToListAsync();

            var game = GameFactory.GetGame();
            var boards = game.Boards.ToList();

            var result = new List<TeamExport>();

            foreach (var team in teams)
            {
                var status = new List<string>();

                if (team.TeamBoards.Any())
                {
                    foreach (var tb in team.TeamBoards)
                    {
                        var board = boards.FirstOrDefault(b => b.Id == tb.BoardId);

                        status.Add(string.Format("{0}: {1} - Start: {2}", board.Name, tb.Score, tb.Start.ToString("g")));
                    }
                }
                else
                {
                    status.Add("Board Not Started");
                }

                var export = new TeamExport
                {
                    Name = team.Name,
                    Organization = team.OrganizationName,
                    Anonymized = TeamExtensions.AnonymizeTeamName(team.OrganizationName, game.AnonymizeTag, team.Number),
                    Created = team.Created.ToString("g"),
                    Locked = team.IsLocked,
                    Members = string.Join(", ", team.Users.Select(u => u.Name)),
                    Status = string.Join(", ", status)
                };

                result.Add(export);
            }

            return result;
        }

        /// <summary>
        /// get team by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TeamSummary> GetSummaryById(string id)
        {
            return Map<TeamSummary>(await Repository.GetById(id));
        }

        /// <summary>
        /// get team by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TeamDetail> GetById(string id)
        {
            return Map<TeamDetail>(await Repository.GetById(id));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="teamId"></param>
        public void CalculateScore(string boardId, string teamId)
        {
            var teamBoard = ProblemRepository.DbContext.TeamBoards.SingleOrDefault(tb => tb.TeamId == teamId && tb.BoardId == boardId);
            if (teamBoard != null)
            {
                var score = ProblemRepository.GetAll()
                    .Where(p => p.TeamId == teamId && p.BoardId == boardId)
                    .Sum(p => p.Score);

                teamBoard.Score = score;
                ProblemRepository.DbContext.SaveChanges();
            }
        }

        /// <summary>
        /// reset team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> Reset(TeamReset model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var team = await Repository.GetById(model.Id);
            team.Badges = string.Empty;
            team.IsLocked = false;
            RemoveUsersFromCache(team);
            await Repository.Update(team);

            return await GetById(model.Id);
        }

        /// <summary>
        /// create new team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> Create(TeamCreate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var user = await DbContext.Users.SingleAsync(u => u.Id == Identity.Id);

            var entity = new Team
            {
                Name = model.Name,
                OwnerUserId = Identity.Id,
                OrganizationLogoUrl = model.OrganizationLogoUrl,
                OrganizationalUnitLogoUrl = model.OrganizationalUnitLogoUrl,
                OrganizationName = model.OrganizationName,
                Number = GetNextOrganizationNumber(model.OrganizationName)
            };

            var saved = await Repository.Add(entity);

            user.TeamId = saved.Id;

            await DbContext.SaveChangesAsync();

            RemoveUsersFromCache(saved);

            return await GetById(saved.Id);
        }

        /// <summary>
        /// update team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> Update(TeamUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var entity = await Repository.GetById(model.Id);
            entity.Name = model.Name;
            entity.OrganizationLogoUrl = model.OrganizationLogoUrl;
            entity.OrganizationalUnitLogoUrl = model.OrganizationalUnitLogoUrl;
            entity.Updated = DateTime.UtcNow;

            if (entity.OrganizationName != model.OrganizationName)
            {
                entity.OrganizationName = model.OrganizationName;
                entity.Number = GetNextOrganizationNumber(model.OrganizationName);
            }
            var saved = await Repository.Update(entity);

            RemoveUsersFromCache(saved);

            return await GetById(saved.Id);
        }

        int GetNextOrganizationNumber(string organizationName)
        {
            var team = DbContext.Teams.Where(t => t.OrganizationName == organizationName).OrderByDescending(t => t.Number).FirstOrDefault();

            return team == null ? 1 : team.Number + 1;
        }

        /// <summary>
        /// add identity to team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> AddUserToTeam(TeamUserUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var team = await DbContext.Teams.SingleOrDefaultAsync(t => t.InviteCode == model.InviteCode);
            var user = await DbContext.Users
                .Include(u => u.Team)
                .SingleOrDefaultAsync(u => u.Id == Identity.Id);

            if (user.Team != null && user.Team.Id != team.Id && user.Team.OwnerUserId == user.Id)
            {
                // if user is the owner of a different team
                // remove members and remove the team

                var members = await DbContext.Users.Where(u => u.TeamId == user.Team.Id).ToListAsync();

                foreach (var member in members)
                {
                    member.TeamId = null;
                }

                DbContext.Teams.Remove(user.Team);
                await DbContext.SaveChangesAsync();
            }

            user.TeamId = team.Id;
            user.Organization = team.OrganizationName;
            await DbContext.SaveChangesAsync();

            UserCache.Remove(user.Id);

            return true;
        }

        /// <summary>
        /// remove user from team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUserFromTeam(TeamUserDelete model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var user = await Repository.DbContext.Users.SingleOrDefaultAsync(u => u.Id == model.UserId);
            user.TeamId = null;
            await Repository.DbContext.SaveChangesAsync();

            UserCache.Remove(user.Id);

            return true;
        }

        /// <summary>
        /// remove user from team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> LeaveTeam(TeamUserLeave model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var user = await Repository.DbContext.Users.SingleOrDefaultAsync(u => u.Id == Identity.Id);
            user.TeamId = null;
            await Repository.DbContext.SaveChangesAsync();

            UserCache.Remove(user.Id);

            return true;
        }

        /// <summary>
        /// lock team
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> Lock(TeamLock model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var team = await Repository.GetById(model.TeamId);
            team.IsLocked = true;
            team.Updated = DateTime.UtcNow;

            await Repository.Update(team);
            RemoveUsersFromCache(team);
            return Map<TeamDetail>(team);
        }

        /// <summary>
        /// generate new invite code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> GenerateInviteCode(TeamInviteCode model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var entity = await Repository.GetById(model.TeamId);
            entity.InviteCode = Guid.NewGuid().ToString("N");
            await DbContext.SaveChangesAsync();
            return entity.InviteCode;
        }

        /// <summary>
        /// start board
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TeamDetail> StartSession(BoardStart model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var board = GameFactory
                .GetGame()
                .Boards
                .SingleOrDefault(b => b.Id == model.Id);

            var teamBoard = new TeamBoard
            {
                BoardId = model.Id,
                TeamId = Identity.User.TeamId
            };

            // this throws on error
            await EngineService.ReserveSession(new GameEngine.Models.SessionRequest
            {
                SessionId = Identity.User.TeamId,
                MaxMinutes = board.MaxMinutes
            });

            if (!await Repository.DbContext.TeamBoards.AnyAsync(tb => tb.TeamId == Identity.User.TeamId && tb.BoardId == model.Id))
            {
                Repository.DbContext.TeamBoards.Add(teamBoard);
                Repository.DbContext.SaveChanges();
            }

            return await GetById(Identity.User.TeamId);
        }

        /// <summary>
        /// reset board session and clear team board
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ResetSession(GameEngineSessionReset model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            await EngineService.CancelSession(Identity.User.TeamId);

            var existingBoards = Repository.DbContext.TeamBoards
                .Where(tb => tb.BoardId == model.BoardId && tb.TeamId == model.TeamId);

            if (existingBoards.Any())
            {
                Repository.DbContext.TeamBoards.RemoveRange(existingBoards);
                Repository.DbContext.SaveChanges();
            }
        }

        /// <summary>
        /// get game related activity by team
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public PagedResult<TeamActivity, TeamActivity> GetActivity(TeamActivityDataFilter dataFilter = null)
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
            {
                throw new EntityPermissionException("User is not a moderator.");
            }

            var boards = GameFactory.GetGame().Boards;

            var nonPracticeBoardIds = boards
                .Where(b => !b.IsPractice)
                .Select(b => b.Id)
                .ToArray();

            var problems = ProblemRepository.GetAll()
                .Where(p => nonPracticeBoardIds.Contains(p.BoardId))
                .ToList();

            var game = GameFactory.GetGame();

            var challenges = game.GetChallenges();

            var teamActivities = new List<TeamActivity>();

            foreach (var p in problems)
            {
                var challenge = challenges.FirstOrDefault(c => c.Id == p.ChallengeLinkId);

                var board = boards.SingleOrDefault(b => b.Id == p.BoardId);

                teamActivities.Add(new TeamActivity
                {
                    Id = p.TeamId,
                    Name = p.Team.Name,
                    Badges = p.Team.Badges.ToBadgeArray(),
                    IsDisabled = p.Team.IsDisabled,
                    Title = challenge.Title,
                    Start = p.Start,
                    End = p.End,
                    Status = p.Status,
                    Score = p.Score,
                    GamespaceReady = p.GamespaceReady,
                    ProblemId = p.Id,
                    BoardId = board.Id,
                    BoardName = board.Name,
                    WorkspaceCode = board.AllowSharedWorkspaces && p.GamespaceReady ? p.SharedId : string.Empty
                });
            }

            return PagedResultFactory.Execute<TeamActivity, TeamActivity>(teamActivities.AsQueryable(), dataFilter, Identity);
        }

        /// <summary>
        /// update team badges
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateBadges(List<TeamBadgeUpdate> model)
        {
            if (!Identity.User.IsModerator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var ids = model.Select(m => m.Id).ToArray();
            var teams = await DbContext.Teams.Where(t => ids.Contains(t.Id)).ToListAsync();

            foreach (var team in teams)
            {
                var update = model.SingleOrDefault(m => m.Id == team.Id);
                if (update == null || !update.Badges.Any())
                {
                    team.Badges = string.Empty;
                }
                else
                {
                    team.Badges = string.Join(" ", update.Badges);
                }

                team.Updated = DateTime.UtcNow;
            }

            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// enable/disable teams
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="isDisabled"></param>
        /// <returns></returns>
        public async Task<bool> SetTeamStatus(List<string> ids, bool isDisabled)
        {
            if (!Identity.User.IsModerator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var teams = await DbContext.Teams.Where(t => ids.Contains(t.Id)).ToListAsync();

            foreach (var team in teams)
            {
                team.Updated = DateTime.UtcNow;
                team.IsDisabled = isDisabled;
            }

            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}

