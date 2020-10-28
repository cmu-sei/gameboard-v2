// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Hubs;
using Gameboard.Integrations;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// user api
    /// </summary>
    [StackAuthorize]
    public class UserController : ApiController<UserService>
    {
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }

        /// <summary>
        /// create an instance of UserController
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="userService"></param>
        /// <param name="logger"></param>
        /// <param name="gameboardHub"></param>
        /// <param name="dataProtector"></param>
        public UserController(
            IStackIdentityResolver identityResolver,
            UserService userService,
            ILogger<UserService> logger,
            IHubContext<GameboardHub, IGameboardEvent> gameboardHub,
            IDataProtectionProvider dataProtector
        ) : base(userService, identityResolver, logger)
        {
            GameboardHub = gameboardHub;
            DataProtector = dataProtector.CreateProtector(
                $"dp:{Assembly.GetEntryAssembly().FullName}"
            );
            Random = new Random();
        }

        IDataProtector DataProtector { get; }
        Random Random { get; }

        /// <summary>
        /// get all users
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/users")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<User, UserDetail>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]UserDataFilter search = null)
        {
            return Ok(await Service.GetAll(search));
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/user/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]string id)
        {
            return Ok(await Service.GetById(id));
        }

        /// <summary>
        /// update user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/user/{id}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]string id, [FromBody]UserEdit model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await Service.Update(model);            

            return Ok(result);
        }

        /// <summary>
        /// toggle moderator role for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/user/{id}/moderator")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> ToggleModerator([FromRoute]string id)
        {
            return Ok(await Service.TogglePermission(id, Permission.Moderator));
        }

        /// <summary>
        /// toggle observer role for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/user/{id}/observer")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> ToggleObserver([FromRoute]string id)
        {
            return Ok(await Service.TogglePermission(id, Permission.Observer));
        }

        /// <summary>
        /// toggle challenge developer role for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/user/{id}/challenge-developer")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> ToggleChallengeDeveloper([FromRoute]string id)
        {
            return Ok(await Service.TogglePermission(id, Permission.ChallengeDeveloper));
        }

        /// <summary>
        /// toggle game designer role for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/user/{id}/game-designer")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> ToggleGameDesigner([FromRoute]string id)
        {
            return Ok(await Service.TogglePermission(id, Permission.GameDesigner));
        }

        /// <summary>
        /// reset user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("api/user/{id}/reset")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(UserDetail), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> Reset([FromRoute]string id)
        {
            return Ok(await Service.Reset(new UserReset { Id = id }));
        }

        /// <summary>
        /// send message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/message")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, Permission.Moderator)]
        public async Task<IActionResult> Message([FromBody]MessageCreate model)
        {
            return Ok(await Service.SendMessage(model));
        }

        /// <summary>
        /// true if survey is complete
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/survey")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> IsSurveyComplete()
        {
            var user = await Service.DbContext.Users.SingleOrDefaultAsync(u => u.Id == Identity.User.Id);
            return Ok(!string.IsNullOrWhiteSpace(user.Survey));
        }

        /// <summary>
        /// submit user survey
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/survey")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Survey([FromBody]List<Answer> model)
        {
            var user = await Service.DbContext.Users.SingleOrDefaultAsync(u => u.Id == Identity.User.Id);            
            user.Survey = JsonConvert.SerializeObject(model);
            await Service.DbContext.SaveChangesAsync();
            return Ok(true);
        }

        /// <summary>
        /// Get one-time auth ticket.
        /// </summary>
        /// <remarks>
        /// Client websocket connections can be authenticated with this ticket
        /// in an `Authorization: Ticket [ticket]` or `Authorization: Bearer [ticket]` header.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("/api/user/ticket")]
        public IActionResult GetTicket()
        {
            int random = Random.Next();

            long expires = DateTime.UtcNow.AddSeconds(20).Ticks;

            string id = $"{User.FindFirstValue(TicketAuthentication.ClaimNames.Subject)}#{User.FindFirstValue(TicketAuthentication.ClaimNames.Name)}";

            string ticket = $"{expires}|{random}|{id}";

            return Ok(new { Ticket = DataProtector.Protect(ticket)});
        }
    }
}

