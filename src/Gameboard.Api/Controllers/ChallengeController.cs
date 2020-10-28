// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Services;
using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// challenge api
    /// </summary>
    [StackAuthorize]
    public class ChallengeController: ApiController<ChallengeService>
    {
        IGameFactory GameFactory { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="challengeService"></param>
        /// <param name="logger"></param>
        /// <param name="gameFactory"></param>
        public ChallengeController(
            IStackIdentityResolver identityResolver,
            ChallengeService challengeService,
            ILogger<ChallengeController> logger,
            IGameFactory gameFactory)
         : base(challengeService, identityResolver, logger) 
        {
            GameFactory = gameFactory;
        }

        /// <summary>
        /// get a challenge
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/challenge/{id}")]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ViewModels.ChallengeProblem), 200)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            return Ok(await Service.Get(new ChallengeRequest { Id = id }));
        }

        /// <summary>
        /// get all tags from challenge specs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/tags")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetTags()
        {
            var result = new List<string>();

            var challengeSpecTags = GameFactory.ChallengeSpecs.Select(c => c.Tags);

            foreach (var challengeSpecTag in challengeSpecTags)
            {
                if (!string.IsNullOrWhiteSpace(challengeSpecTag))
                {
                    result.AddRange(challengeSpecTag.ToLower().Split(' '));
                }
            }

            result = result.Distinct().OrderBy(x => x).ToList();

            return Ok(result);
        }

        /// <summary>
        /// get player challenge report
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/report/challenge-tags")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ChallengeTagReport), 200)]
        [StackAuthorize]
        public async Task<IActionResult> GetChallengeTagReport()
        {
            return Ok(await Service.GetChallengeTagReport());
        }

        /// <summary>
        /// get all challenge specs
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/spec/challenges")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<ChallengeSpec, ChallengeSpec>), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.ChallengeDeveloper)]
        public async Task<IActionResult> GetAllChallengeSpecs([FromQuery]ChallengeSpecDataFilter search = null)
        {
            return Ok(await Service.GetAllChallengeSpecs(search));
        }

        /// <summary>
        /// get challenge by slug
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("api/spec/challenge/{slug}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ChallengeSpec), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.ChallengeDeveloper)]
        public async Task<IActionResult> GetChallengeSpec([FromRoute]string slug)
        {
            return Ok(await Service.GetChallengeSpec(slug));
        }

        /// <summary>
        /// add challenge spec
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/spec/challenges")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ChallengeSpec), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.ChallengeDeveloper)]
        public async Task<IActionResult> AddChallengeSpec([FromBody]ChallengeSpec model)
        {
            return Ok(await Service.AddChallengeSpec(model));
        }

        /// <summary>
        /// update challenge spec
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/spec/challenge/{slug}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ChallengeSpec), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.ChallengeDeveloper)]
        public async Task<IActionResult> UpdateChallengeSpec([FromRoute]string slug, [FromBody]ChallengeSpec model)
        {
            return Ok(await Service.UpdateChallengeSpec(slug, model));
        }

        /// <summary>
        /// delete challenge spec
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpDelete("api/spec/challenge/{slug}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.ChallengeDeveloper)]
        public async Task<IActionResult> DeleteChallengeSpec([FromRoute]string slug)
        {
            await Service.DeleteChallengeSpec(slug);

            return Ok(true);
        }

        /// <summary>
        /// get a challenge
        /// </summary>
        /// <param name="id"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [HttpGet("api/challenge/{id}/team/{teamId}")]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ViewModels.ChallengeProblem), 200)]
        public async Task<IActionResult> Get([FromRoute] string id, [FromRoute]string teamId)
        {
            return Ok(await Service.Get(new ChallengeRequest { Id = id, TeamId = teamId }));
        }

        /// <summary>
        /// check if challenge survey is complete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/challenge/{id}/survey")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> IsSurveyComplete([FromRoute]string id)
        {
            var game = GameFactory.GetGame();

            var challenge = game.FindChallengeByChallengeLinkId(id);

            if (challenge == null)
                throw new EntityNotFoundException($"Challenge '{id}' was not found.");

            var survey = await Service.DbContext.Surveys.SingleOrDefaultAsync(x => x.UserId == Identity.User.Id && x.ChallengeId == challenge.Id);

            if (survey == null)
                return Ok(false);

            return Ok(!string.IsNullOrWhiteSpace(survey.Data));
        }

        /// <summary>
        /// add challenge survey
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/challenge/{id}/survey")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Survey([FromRoute]string id, [FromBody]List<Answer> model)
        {
            var game = GameFactory.GetGame();

            var challenge = game.FindChallengeByChallengeLinkId(id);

            if (challenge == null)
                throw new EntityNotFoundException($"Challenge '{id}' was not found.");

            var db = Service.DbContext;
            var survey = await db.Surveys.SingleOrDefaultAsync(x => x.UserId == Identity.User.Id && x.ChallengeId == id);

            if (survey == null)
            {
                survey = new Survey { ChallengeId = id, UserId = Identity.User.Id };
                await db.Surveys.AddAsync(survey);
            }

            survey.Data = JsonConvert.SerializeObject(model);
            await db.SaveChangesAsync();

            return Ok(true);
        }
    }
}

