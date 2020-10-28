// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// leaderboard api
    /// </summary>
    [StackAuthorize]
    public class LeaderboardController : ApiController<LeaderboardService>
    {
        BoardService BoardService { get; }
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; set; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="service"></param>
        /// <param name="boardService"></param>
        /// <param name="logger"></param>
        /// <param name="hub"></param>
        public LeaderboardController(IStackIdentityResolver resolver, LeaderboardService service, BoardService boardService, ILogger<LeaderboardController> logger, IHubContext<GameboardHub, IGameboardEvent> hub)
            : base(service, resolver, logger)
        {
            BoardService = boardService;
            GameboardHub = hub;

        }

        /// <summary>
        /// get loaderboard by board
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api/leaderboard/{boardId}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(Leaderboard), 200)]
        public async Task<IActionResult> Get([FromRoute]string boardId, [FromQuery]LeaderboardDataFilter search = null)
        {
            return Ok(await Service.Get(boardId, search));
        }

        /// <summary>
        /// export leaderboard by board id
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        [HttpGet("api/leaderboard/{boardId}/export")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator)]
        public IActionResult Export([FromRoute]string boardId)
        {
            var result = Service.Export(boardId);

            return File(
                Service.ConvertToBytes(result),
                "application/octet-stream",
                string.Format("leaderboard-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }

        /// <summary>
        /// get team score
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("api/leaderboard/{boardId}/team/{teamId}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(LeaderboardScore), 200)]
        public IActionResult GetTeamScore([FromRoute]string boardId, [FromRoute]string teamId)
        {
            return Ok(Service.GetTeamScore(boardId, teamId));
        }

        /// <summary>
        /// recalculate leaderboard
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/leaderboard")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public IActionResult RefreshLeaderboard()
        {
            if (!Service.Identity.User.IsModerator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var leaderboards = Service.Calculate();

            if (leaderboards.Any())
            {
                foreach (var leaderboard in leaderboards)
                {
                    var gameboardEvent = GameboardHub.Clients.All;
                    gameboardEvent.LeaderboardUpdated(leaderboard).Wait();
                }
            }

            return Ok(true);
        }
    }
}

