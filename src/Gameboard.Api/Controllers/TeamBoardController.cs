// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// team board api
    /// </summary>
    [StackAuthorize]
    public class TeamBoardController : ApiController<TeamBoardService>
    {
        LeaderboardService LeaderboardService { get; }
        TeamService TeamService { get; }
        ProblemService ProblemService { get; }
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="teamBoardService"></param>
        /// <param name="teamService"></param>
        /// <param name="problemService"></param>
        /// <param name="leaderboardService"></param>
        /// <param name="logger"></param>
        /// <param name="hub"></param>
        public TeamBoardController(IStackIdentityResolver identityResolver, TeamBoardService teamBoardService,
            TeamService teamService,
            ProblemService problemService,
            LeaderboardService leaderboardService, 
            ILogger<TeamBoardController> logger, 
            IHubContext<GameboardHub, IGameboardEvent> hub)
            : base(teamBoardService, identityResolver, logger)
        {
            LeaderboardService = leaderboardService;
            TeamService = teamService;
            ProblemService = problemService;
            GameboardHub = hub;
        }

        /// <summary>
        /// update team board
        /// </summary>
        /// <remarks>model should be expanded for additional updates</remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/teamboard")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> Update([FromBody]TeamBoardUpdate model)
        {
            if (!Service.Identity.User.IsModerator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var result = await Service.OverrideTimeRemaining(model);

            await GameboardHub.Clients.Group(result.Id).TeamUpdated(result);

            // update the leaderboard to reflect the changes
            var leaderboards = LeaderboardService.Calculate();

            if (leaderboards.Any())
            {
                foreach (var leaderboard in leaderboards)
                {
                    var gameboardEvent = GameboardHub.Clients.All;
                    gameboardEvent.LeaderboardUpdated(leaderboard).Wait();
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// get teamboard by board id
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        [HttpGet("api/teamboard/{boardId}/score")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamBoardDetail), 200)]
        public async Task<IActionResult> GetScore([FromRoute]string boardId)
        {
            return Ok(await Service.GetScore(boardId));
        }

        /// <summary>
        /// get team board events
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("api/teamboard/{teamId}/events")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamBoardEventDetail[]), 200)]
        public async Task<IActionResult> GetTeamBoardEvents([FromRoute]string teamId)
        {
            return Ok(await Service.GetTeamBoardEvents(teamId));
        }

        /// <summary>
        /// reset team board and all gameplay data 
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpPost("api/board/{boardId}/team/{teamId}/reset")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> Reset([FromRoute]string boardId, [FromRoute]string teamId)
        {
            await ProblemService.ResetChallenges(new TeamBoardReset { BoardId = boardId, TeamId = teamId });
            await TeamService.ResetSession(new GameEngineSessionReset { BoardId = boardId, TeamId = teamId });

            var team = await TeamService.GetById(teamId);

            await GameboardHub.Clients.Group(teamId).TeamUpdated(team);
            await GameboardHub.Clients.Group(teamId).BoardReset(team);

            return Ok(team);
        }

    }
}

