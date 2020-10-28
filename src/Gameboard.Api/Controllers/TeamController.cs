// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// team api
    /// </summary>
    [StackAuthorize]
    public class TeamController : ApiController<TeamService>
    {
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }
        OrganizationOptions OrganizationOptions { get; }
        ProblemService ProblemService { get; }

        /// <summary>
        /// create an instance of team controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="teamService"></param>
        /// <param name="problemService"></param>
        /// <param name="logger"></param>
        /// <param name="hub"></param>
        /// <param name="organizationOptions"></param>
        public TeamController(
            IStackIdentityResolver identityResolver,
            TeamService teamService,
            ProblemService problemService,
            ILogger<TeamService> logger,
            IHubContext<GameboardHub, IGameboardEvent> hub,
            OrganizationOptions organizationOptions)
            : base(teamService, identityResolver, logger)
        {
            GameboardHub = hub;
            ProblemService = problemService;
            OrganizationOptions = organizationOptions;
        }

        /// <summary>
        /// get all teams
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/teams")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Team, TeamDetail>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]TeamDataFilter search = null)
        {
            return Ok(await Service.GetAll(search));
        }

        /// <summary>
        /// export teams
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/teams/export")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> ExportTeams()
        {
            var result = await Service.ExportTeams();

            return File(
                Service.ConvertToBytes(result),
                "application/octet-stream",
                string.Format("team-export-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }

        /// <summary>
        /// get team by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/team/{id}/summary")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamSummary), 200)]
        public async Task<IActionResult> GetSummaryById([FromRoute]string id)
        {
            return Ok(await Service.GetSummaryById(id));
        }

        /// <summary>
        /// get team by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/team/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]string id)
        {
            return Ok(await Service.GetById(id));
        }

        /// <summary>
        /// create team
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/teams")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> Create([FromBody]TeamCreate model)
        {
            var result = await Service.Create(model);
            return Created("~/api/team/" + result.Id, result);
        }

        /// <summary>
        /// update team
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/team/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]string id, [FromBody]TeamUpdate model)
        {
            var team = await Service.Update(model);

            await GameboardHub.Clients.Group(id).TeamUpdated(new TeamDetail
            {
                Id = team.Id,
                Name = team.Name,
                OrganizationName = team.OrganizationName,
                IsLocked = team.IsLocked,
                OwnerUserId = team.OwnerUserId,
                OrganizationLogoUrl = team.OrganizationLogoUrl,
                OrganizationalUnitLogoUrl = team.OrganizationalUnitLogoUrl,
                Number = team.Number,
                Created = team.Created,
            });

            return Ok(true);
        }

        /// <summary>
        /// reset team and all gameplay data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/team/{id}/reset")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> Reset([FromRoute]string id)
        {
            var team = await Service.GetById(id);

            foreach (var tb in team.TeamBoards)
            {
                await ProblemService.ResetChallenges(new TeamBoardReset { BoardId = tb.Board.Id, TeamId = tb.TeamId });
                await Service.ResetSession(new GameEngineSessionReset { BoardId = tb.Board.Id, TeamId = tb.TeamId });
            }

            team = await Service.Reset(new TeamReset() { Id = id });

            await GameboardHub.Clients.Group(id).TeamUpdated(team);
            await GameboardHub.Clients.Group(id).BoardReset(team);

            return Ok(team);
        }

        /// <summary>
        /// join team with valid invite code
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPut("api/team/{id}/join/{code}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Join([FromRoute]string id, [FromRoute]string code)
        {
            return Ok(await Service.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = id }));
        }

        /// <summary>
        /// leave team
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/team/{id}/leave")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Leave([FromRoute]string id)
        {
            await Service.LeaveTeam(new TeamUserLeave { TeamId = id });

            await GameboardHub.Clients.Group(id).PresenceUpdated(new MemberPresence
            {
                Id = Identity.Id,
                TeamId = id,
                EventType = PresenceEvent.Kicked
            });
            return Ok(true);
        }

        /// <summary>
        /// generate new 6 digit invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/team/{id}/code")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamInviteCode), 200)]
        public async Task<IActionResult> GenerateInviteCode([FromRoute]string id)
        {
            var model = new TeamInviteCode { TeamId = id };
            model.InvitationCode = await Service.GenerateInviteCode(model);
            return Ok(model);
        }

        /// <summary>
        /// remove user from team
        /// </summary>
        /// <remarks>must be team owner</remarks>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("api/team/{id}/user/{userId}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> RemoveUser([FromRoute]string id, [FromRoute]string userId)
        {
            await Service.RemoveUserFromTeam(new TeamUserDelete { UserId = userId, TeamId = id });

            await GameboardHub.Clients.Group(id).PresenceUpdated(new MemberPresence
            {
                Id = userId,
                TeamId = id,
                EventType = PresenceEvent.Kicked
            });
            return Ok(true);
        }

        /// <summary>
        /// lock team
        /// </summary>
        /// <remarks>must be team owner</remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/team/{id}/lock")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Lock([FromRoute]string id)
        {
            var team = await Service.Lock(new TeamLock { TeamId = id });
            await GameboardHub.Clients.Group(id).TeamUpdated(team);
            return Ok(true);
        }

        /// <summary>
        /// get game related activity
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/teams/activity")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<TeamActivity, TeamActivity>), 200)]
        public IActionResult GetActivity([FromQuery]TeamActivityDataFilter search = null)
        {
            return Ok(Service.GetActivity(search));
        }

        /// <summary>
        /// export game releasted activity
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/teams/activity/export")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public IActionResult ExportActivity()
        {
            var result = Service.GetActivity(null);

            return File(
                Service.ConvertToBytes(result.Results),
                "application/octet-stream",
                string.Format("team-activity-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }

        /// <summary>
        /// send a message to all logged in members of all teams
        /// </summary>
        /// <remarks>must be a moderator</remarks>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPut("api/team/sendmessage")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> SendMessage([FromBody]string message)
        {
            await GameboardHub.Clients.All.SystemMessage(message);
            return Ok(true);
        }

        /// <summary>
        /// update team badges
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/teams/badges")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> UpdateBadges([FromBody]List<TeamBadgeUpdate> model)
        {
            var result = await Service.UpdateBadges(model);

            foreach (var t in model)
            {
                var team = await Service.GetById(t.Id);
                await GameboardHub.Clients.Group(t.Id).TeamUpdated(team);
            }

            return Ok(result);
        }

        /// <summary>
        /// disable teams
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPut("api/teams/disable")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Disable([FromBody]List<string> ids)
        {
            return Ok(await Service.SetTeamStatus(ids, true));
        }

        /// <summary>
        /// enable teams
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPut("api/teams/enable")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Enable([FromBody]List<string> ids)
        {
            return Ok(await Service.SetTeamStatus(ids, false));
        }

        /// <summary>
        /// get all organizations
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/organizations")]
        [JsonExceptionFilter]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<OrganizationDetail>), 200)]
        public IActionResult Organizations()
        {
            var organizations = (OrganizationOptions.Items ?? new List<OrganizationDetail>())
                .OrderBy(o => o.Title);

            return Ok(organizations);
        }
    }
}

