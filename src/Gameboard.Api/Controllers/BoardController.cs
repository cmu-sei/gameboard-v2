// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// board api
    /// </summary>
    [StackAuthorize]
    public class BoardController : ApiController<BoardService>
    {
        TeamService TeamService { get; }
        ProblemService ProblemService { get; }
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }
        IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="boardService"></param>
        /// <param name="logger"></param>
        /// <param name="teamService"></param>
        /// <param name="hub"></param>
        /// <param name="problemService"></param>
        /// <param name="hostingEnvironment"></param>
        public BoardController(
            IStackIdentityResolver identityResolver,
            BoardService boardService,            
            ILogger<BoardController> logger,
            TeamService teamService,
            IHubContext<GameboardHub, IGameboardEvent> hub,
            ProblemService problemService,
            IHostingEnvironment hostingEnvironment
        ) : base(boardService, identityResolver, logger)
        {
            TeamService = teamService;
            ProblemService = problemService;
            GameboardHub = hub;
            HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// get all boards
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/boards")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(BoardDetail[]), 200)]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(Service.GetAll());
        }

        /// <summary>
        /// get board by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/board/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(BoardDetail), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            return Ok(await Service.Get(new BoardRequest { Id = id }, Identity?.User?.TeamId));
        }

        /// <summary>
        /// get board by team
        /// </summary>
        /// <param name="id"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("api/board/{id}/team/{teamId}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(BoardDetail), 200)]
        public async Task<IActionResult> GetBoardByTeam([FromRoute] string id, [FromRoute] string teamId)
        {
            return Ok(await Service.Get(new BoardRequest { Id = id }, teamId));
        }

        /// <summary>
        /// get team board status for active user team
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/board/{id}/status")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamBoardStatus), 200)]
        public async Task<IActionResult> Status([FromRoute] string id)
        {
            return Ok(await Service.GetSessionForecast(id));
        }

        /// <summary>
        /// start board
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/board/{id}/start")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> Start([FromRoute] string id)
        {
            var model = new BoardStart
            {
                Id = id
            };

            var team = await TeamService.StartSession(model);
            await GameboardHub.Clients.Group(Identity.User.TeamId).TeamUpdated(team);
            return Ok(team);
        }

        /// <summary>
        /// get completion report
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/report/completion")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(BoardCompletionReport[]), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator, Permission.Observer)]
        public async Task<IActionResult> GetBoardCompletionReports()
        {
            return Ok(await Service.GetBoardCompletionReports());
        }

        /// <summary>
        /// get completion report by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/report/completion/board/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(BoardCompletionReport), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator, Permission.Observer)]
        public async Task<IActionResult> GetBoardCompletionReport([FromRoute]string id)
        {
            return Ok(await Service.GetBoardCompletionReport(id));
        }

        /// <summary>
        /// export completion report by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/report/completion/board/{id}/export")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator, Permission.Observer)]
        public async Task<IActionResult> ExportBoardCompletionReport([FromRoute]string id)
        {
            var result = await Service.ExportBoardCompletionReport(id);

            return File(
                Service.ConvertToBytes(result),
                "application/octet-stream",
                string.Format("completion-report-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }

        /// <summary>
        /// generate a challenge survey report by board
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/report/challenge-survey/board/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ChallengeSurveyReport), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator)]
        public async Task<IActionResult> GetChallengeSurveyReport([FromRoute]string id)
        {
            return Ok(await Service.GetChallengeSurveyReport(id));
        }

        /// <summary>
        /// export challenge survey report
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/report/challenge-survey/board/{id}/export")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAny, Permission.Moderator)]
        public async Task<IActionResult> ExportChallengeSurveyReport([FromRoute]string id)
        {
            var result = await Service.GetChallengeSurveyReport(id);

            return File(
                Service.ConvertToBytes(result.Items),
                "application/octet-stream",
                string.Format("challenge-survey-report-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".csv");
        }

        /// <summary>
        /// reset board for current user's team
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/board/{id}/reset")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TeamDetail), 200)]
        public async Task<IActionResult> Reset([FromRoute] string id)
        {
            await ProblemService.ResetChallenges(new TeamBoardReset { BoardId = id, TeamId = Identity.User.TeamId });
            await TeamService.ResetSession(new GameEngineSessionReset { BoardId = id, TeamId = Identity.User.TeamId });

            var team = await TeamService.GetById(Identity.User.TeamId);

            await GameboardHub.Clients.Group(Identity.User.TeamId).BoardReset(team);

            return Ok(team);
        }

        /// <summary>
        /// download certificate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/board/{id}/certificate")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> Certificate([FromRoute] string id)
        {
            var teamBoard = await Service.GetTeamBoard(id);

            if (teamBoard == null)
                throw new InvalidModelException("Team has not started this board.");

            if (teamBoard.Board.CertificateThreshold <= 0)
                throw new InvalidModelException("This board does not have a certificate threshold.");

            if (teamBoard.Score < teamBoard.Board.CertificateThreshold)
                throw new InvalidModelException("Team did not meet the board certificate threshold.");

            string userName = Identity.User.Name;
            string date = DateTime.Now.ToString("M/d/yyyy");
            string points = teamBoard.Score.ToString();

            string gameName = Service.GameFactory.GetGame().Name;
            string boardName = teamBoard.Board.Name;

            var nameLocation = new PointF(250f, 440f);
            var dateLocation = new PointF(650f, 490f);
            var pointLocation = new PointF(250f, 500f);

            var gameLocation = new PointF(100f, 240f);
            var boardLocation = new PointF(605f, 240f);

            var imageFilePath = Path.Combine(HostingEnvironment.ContentRootPath, "_content", "2019PC-certificate-white_score.png");
            
            var bitmap = (Bitmap)Image.FromFile(imageFilePath);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                using (var arialFont = new Font("Arial", 14))
                {
                    graphics.DrawString(userName, arialFont, Brushes.Black, nameLocation);
                    graphics.DrawString(date, arialFont, Brushes.Black, dateLocation);
                    graphics.DrawString(points, arialFont, Brushes.Black, pointLocation);
                }

                using (var arialFont = new Font("Arial", 20))
                {
                    graphics.DrawString(gameName, arialFont, Brushes.Black, gameLocation);
                    graphics.DrawString(boardName, arialFont, Brushes.Black, boardLocation);
                }
            }

            byte[] bytes = null;

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                bytes = stream.ToArray();
            }

            var fileName = string.Format("certificate-{0}", DateTime.UtcNow.ToString("yyyy-MM-dd")) + ".png";

            return File(bytes, "image/png", fileName);
        }
    }
}

