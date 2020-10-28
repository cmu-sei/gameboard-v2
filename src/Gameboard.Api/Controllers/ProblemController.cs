// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// problem api
    /// </summary>
    [StackAuthorize]
    public class ProblemController : ApiController<ProblemService>
    {
        LeaderboardService LeaderboardService { get; }
        IHubContext<GameboardHub, IGameboardEvent> Hub { get; }
        TeamService TeamService { get; }
        IGameFactory GameFactory { get; }

        /// <summary>
        /// create an instance of problem controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="problemService"></param>        
        /// <param name="teamService"></param>
        /// <param name="leaderboardService"></param>
        /// <param name="logger"></param>
        /// <param name="hub"></param>
        /// <param name="gameFactory"></param>
        public ProblemController(
            IStackIdentityResolver identityResolver,
            ProblemService problemService,            
            TeamService teamService,
            LeaderboardService leaderboardService,
            ILogger<ProblemService> logger,
            IHubContext<GameboardHub, IGameboardEvent> hub,
            IGameFactory gameFactory
        )
            : base(problemService, identityResolver, logger)
        {
            Hub = hub;
            GameFactory = gameFactory;
            LeaderboardService = leaderboardService;
            TeamService = teamService;
        }

        /// <summary>
        /// start challenge
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/challenge/start")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProblemDetail), 200)]
        public async Task<IActionResult> Start([FromBody]ChallengeStart model)
        {
            var result = await Service.Create(model);
            return Created("~/api/problem/" + result.Id, result);
        }

        /// <summary>
        /// reset challenge in test mode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/challenge/{id}/reset")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Reset([FromRoute]string id)
        {
            var model = new ChallengeReset
            {
                ChallengeLinkId = id,
                TeamId = Identity?.User?.TeamId
            };

            await Service.ResetChallenge(model);
            await RecalculateAndNotifyTeam(Identity?.User?.TeamId, id);

            return Ok(true);
        }

        /// <summary>
        /// restart challenge in errored state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/challenge/{id}/restart")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Restart([FromRoute]string id)
        {
            var model = new ChallengeRestart
            {
                ChallengeLinkId = id,
                TeamId = Identity?.User?.TeamId
            };

            await Service.RestartChallenge(model);
            await RecalculateAndNotifyTeam(Identity?.User?.TeamId, id);

            return Ok(true);
        }

        /// <summary>
        /// recalculate scores and notify hub clients
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="challengeLinkId"></param>
        /// <returns></returns>
        async Task RecalculateAndNotifyTeam(string teamId, string challengeLinkId)
        {
            if (teamId != null)
            {
                var game = GameFactory.GetGame();
                var board = game.Boards.FindByChallengeLinkId(challengeLinkId);
                TeamService.CalculateScore(board.Id, teamId);

                var team = await TeamService.GetById(teamId);
                await Hub.Clients.Group(teamId).TeamUpdated(team);
            }

            LeaderboardService.Calculate();
        }
        /// <summary>
        /// get vm ticket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vmId"></param>
        /// <returns></returns>
        [HttpGet("api/problem/{id}/console/{vmId}")]
        [JsonExceptionFilter]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ProblemConsoleSummary>> GetVmTicket([FromRoute]string id, [FromRoute]string vmId)
        {
            //the id value can contain either a problem id or shared id
            return Ok(await Service.GetTicket(new ProblemConsole
            {
                Id = id,
                VmId = vmId
            }));
        }

        /// <summary>
        /// change vm
        /// </summary>
        /// <param name="vmAction"></param>
        /// <returns></returns>
        [HttpPut("api/console/vmaction")]
        [JsonExceptionFilter]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ProblemConsoleSummary>> ChangeVm([FromBody]ProblemConsoleAction vmAction)
        {
            await Service.ChangeVm(vmAction);
            return Ok();
        }

        /// <summary>
        /// restart game space
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/problem/{id}/gamespace")]
        [JsonExceptionFilter]
        [ProducesResponseType(200)]
        public async Task<ActionResult> RestartGamespace([FromRoute]string id)
        {
            await Service.RestartGamespace(new GamespaceRestart { ProblemId = id });
            return Ok();
        }

        /// <summary>
        /// delete game space
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/problem/{id}/gamespace")]
        [JsonExceptionFilter]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteGamespace([FromRoute]string id)
        {
            await Service.DeleteGamespace(new GamespaceDelete { Id = id });
            return Ok();
        }

        /// <summary>
        /// get all gamespaces
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/gamespaces")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Problem, GamespaceDetail>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ProblemDataFilter search = null)
        {
            return Ok(await Service.GetGamespaces(search));
        }
    }
}

