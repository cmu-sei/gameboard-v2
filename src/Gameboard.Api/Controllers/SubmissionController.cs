// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System.Threading.Tasks;
using Gameboard.ViewModels;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// Submission api endpoints
    /// </summary>
    [StackAuthorize]
    public class SubmissionController : ApiController<SubmissionService>
    {
        IHubContext<GameboardHub, IGameboardEvent> Hub { get; }
        TeamService TeamService { get; set; }
        ProblemService ProblemService { get; set; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="submissionService"></param>
        /// <param name="problemService"></param>
        /// <param name="teamService"></param>
        /// <param name="logger"></param>
        /// <param name="hub"></param>
        public SubmissionController(
            IStackIdentityResolver identityResolver,
            SubmissionService submissionService,
            ProblemService problemService,
            TeamService teamService,
            ILogger<SubmissionService> logger,
            IHubContext<GameboardHub, IGameboardEvent> hub)
            : base(submissionService, identityResolver, logger)
        {
            Hub = hub;
            TeamService = teamService;
            ProblemService = problemService;
        }

        /// <summary>
        /// create submission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/submissions")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(SubmissionDetail), 200)]
        public async Task<IActionResult> Create([FromBody]SubmissionCreate model)
        {
            var result = await Service.Create(model);

            var problem = await ProblemService.GetById(result.ProblemId);
            var team = await TeamService.GetById(problem.TeamId);

            await Hub.Clients.Group(problem.TeamId).ProblemUpdated(problem);            
            await Hub.Clients.Group(problem.Id).TeamUpdated(team);

            return Created("~/api/submission/" + result.Id, result);
        }
    }
}

