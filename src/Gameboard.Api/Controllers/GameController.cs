// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// game api
    /// </summary>
    [StackAuthorize]
    public class GameController : ApiController
    {
        GameboardDbContext DbContext { get; }        
        IGameFactory GameFactory { get; }
        GameService GameService { get; }
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="logger"></param>
        /// <param name="dbContext"></param>
        /// <param name="gameFactory"></param>
        /// <param name="hub"></param>
        /// <param name="gameService"></param>
        public GameController(
            IStackIdentityResolver identityResolver,
            ILogger<BoardController> logger,
            GameboardDbContext dbContext,
            IGameFactory gameFactory,
            IHubContext<GameboardHub, IGameboardEvent> hub,
            GameService gameService
        )
            : base(identityResolver, logger)
        {
            DbContext = dbContext;
            GameFactory = gameFactory;
            GameboardHub = hub;
            GameService = gameService;
        }

        /// <summary>
        /// get participation report
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/report/participation")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ParticipationReport), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> GetParticipationReport()
        {
            var game = GameFactory.GetGame();

            var report = new ParticipationReport
            {
                TeamRegistrantCount = await DbContext.Users.CountAsync(u => !string.IsNullOrWhiteSpace(u.TeamId)),
                IndividualRegistrantCount = await DbContext.Users.CountAsync(u => string.IsNullOrWhiteSpace(u.TeamId)),
                TeamCount = await DbContext.Teams.CountAsync(),
                Text = game.MaxTeamSize > 1 ? "Teams" : "Players",
                Organizations = await DbContext.Teams
                    .GroupBy(t => t.OrganizationName)
                    .OrderBy(t => t.Key)
                    .Select(x => new ParticipationReportOrganization { Name = x.Key, TeamCount = x.Count() })
                    .ToListAsync()
            };

            return Ok(report);
        }        

        /// <summary>
        /// get game without board information
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/game")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(GameDetail), 200)]
        [AllowAnonymous]
        public IActionResult Detail()
        {
            var game = GameFactory.GetGame();

            var detail = new GameDetail
            {
                Id = game.Id,
                Name = game.Name,
                MaxTeamSize = game.MaxTeamSize,
                MinTeamSize = game.MinTeamSize,
                EnrollmentEndsAt = game.EnrollmentEndsAt,
                StartTime = game.StartTime,
                StopTime = game.StopTime,
                MaxConcurrentProblems = game.MaxConcurrentProblems
            };

            return Ok(detail);
        }

        /// <summary>
        /// get all games
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/games")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<GameDetail, GameDetail>), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.GameDesigner)]
        public async Task<IActionResult> GetAll([FromQuery]GameDataFilter search = null)
        {
            return Ok(await GameService.GetAll(search));
        }

        /// <summary>
        /// get game by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/game/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(GameDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.GameDesigner)]
        public async Task<IActionResult> Get([FromRoute]string id)
        {
            return Ok(await GameService.GetById(id));
        }

        /// <summary>
        /// create game
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/games")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(GameDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.GameDesigner)]
        public async Task<IActionResult> Create([FromBody]GameEdit model)
        {
            var game = await GameService.Create(model);
            await GameboardHub.Clients.All.GameUpdated(game);
            return Ok(game);
        }

        /// <summary>
        /// update game
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/game/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(GameDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.GameDesigner)]
        public async Task<IActionResult> Update(string id, [FromBody]GameEdit model)
        {
            var game = await GameService.Update(id, model);
            await GameboardHub.Clients.All.GameUpdated(game);
            return Ok(game);
        }

        /// <summary>
        /// reload game
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/game/reload")]        
        [JsonExceptionFilter]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.GameDesigner)]
        public async Task<IActionResult> Reload()
        {
            await GameService.Refresh();
            await GameboardHub.Clients.All.GameUpdated(GameFactory.GetGame());
            return Ok();
        }

        /// <summary>
        /// generate a game survey
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/report/survey")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(SurveyReport), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator)]
        public async Task<IActionResult> GetSurveyReport()
        {
            return Ok(await GameService.GetSurveyReport());
        }

        /// <summary>
        /// export survey report
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/report/survey/export")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator)]
        public async Task<IActionResult> ExportChallengeSurveyReport()
        {
            var result = await GameService.GetSurveyReport();

            return File(
                GameService.ConvertToBytes(result.Items),
                "application/octet-stream",
                string.Format("survey-report-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }
    }
}

