// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Api.Models;
using Gameboard.Repositories;
using Gameboard.ViewModels;
using GameEngine.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stack.Data.Options;
using Stack.DomainEvents;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Http.Models;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// home index and api
    /// </summary>
    [StackAuthorize]
    public class HomeController : Controller
    {
        BrandingOptions BrandingOptions { get; }
        DatabaseOptions DatabaseOptions { get; }
        CachingOptions CachingOptions { get; }
        GameEngine.Client.Options GameEngineOptions { get; }
        IGameFactory GameFactory { get; }
        IStackIdentityResolver IdentityResolver { get; }
        Stack.Http.Options.AuthorizationOptions AuthorizationOptions { get; }
        DomainEventDispatcherOptions DomainEventDispatcherOptions { get; }
        EnvironmentOptions EnvironmentOptions { get; }

        /// <summary>
        /// create an instance of home controller
        /// </summary>
        /// <param name="brandingOptions"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="cachingOptions"></param>
        /// <param name="gameEngineOptions"></param>
        /// <param name="gameFactory"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="domainEventDispatcherOptions"></param>
        /// <param name="environmentOptions"></param>
        /// <param name="identityResolver"></param>
        public HomeController(
            BrandingOptions brandingOptions,
            DatabaseOptions databaseOptions,
            CachingOptions cachingOptions,
            GameEngine.Client.Options gameEngineOptions,
            IGameFactory gameFactory,
            Stack.Http.Options.AuthorizationOptions authorizationOptions,
            DomainEventDispatcherOptions domainEventDispatcherOptions,
            EnvironmentOptions environmentOptions,
            IStackIdentityResolver identityResolver)
        {
            BrandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            DatabaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            CachingOptions = cachingOptions ?? throw new ArgumentNullException(nameof(cachingOptions));
            GameEngineOptions = gameEngineOptions ?? throw new ArgumentNullException(nameof(gameEngineOptions));
            GameFactory = gameFactory ?? throw new ArgumentNullException(nameof(gameFactory));
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            EnvironmentOptions = environmentOptions ?? throw new ArgumentNullException(nameof(environmentOptions));
            DomainEventDispatcherOptions = domainEventDispatcherOptions;
        }

        /// <summary>
        /// is enrollment allowed
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/isEnrollmentAllowed")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        [AllowAnonymous]
        public IActionResult IsEnrollmentAllowed()
        {
            var game = GameFactory.GetGame();

            var isEnrollmentAllowed = game.EnrollmentEndsAt.HasValue
                && DateTime.UtcNow < game.EnrollmentEndsAt;

            return Ok(isEnrollmentAllowed);
        }

        /// <summary>
        /// enrollment ends at
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/enrollmentEndsAt")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(string), 200)]
        [AllowAnonymous]
        public IActionResult EnrollmentEndsAt()
        {
            var game = GameFactory.GetGame();

            if (game.EnrollmentEndsAt.HasValue)
                return Ok(game.EnrollmentEndsAt.Value.ToString("G"));

            return Ok(null);
        }

        /// <summary>
        /// root
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = new HomeModel
            {
                ApplicationName = BrandingOptions.ApplicationName,
                ApiStatus = new ApiStatus("Gameboard"),
                Configuration = GetConfiguration()
            };

            return View(model);
        }

        /// <summary>
        /// error page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("~/Views/Shared/Error.cshtml", feature?.Error);
        }

        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api/status")]
        [ProducesResponseType(typeof(StatusDetail), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Status()
        {
            var status = new StatusDetail
            {
                Status = new ApiStatus("Gameboard", "GameEngine.Client", "GameEngine.Abstractions"),
                Commit = Environment.GetEnvironmentVariable("COMMIT")
            };

            return Ok(status);
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api/configuration")]
        [ProducesResponseType(typeof(List<ConfigurationItem>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Configuration()
        {
            var items = GetConfiguration();
            return Ok(items);
        }

        List<ConfigurationItem> GetConfiguration()
        {
            var items = new List<ConfigurationItem>
            {
                new ConfigurationItem("Database", new Dictionary<string, object> {
                    { "Provider", DatabaseOptions.Provider },
                    { "Auto Migrate", DatabaseOptions.AutoMigrate },
                    { "Dev Mode Recreate", DatabaseOptions.DevModeRecreate }
                }),
                new ConfigurationItem("Authorization", new Dictionary<string, object> {
                    { "Authority", AuthorizationOptions.Authority },
                    { "Scope", AuthorizationOptions.AuthorizationScope },
                    { "Client Id", AuthorizationOptions.ClientId },
                    { "Client Name", AuthorizationOptions.ClientName }
                }),
                new ConfigurationItem("Caching", new Dictionary<string, object> {
                    { "Type", CachingOptions.CacheType },
                    { "SlidingExpirationMinutes", CachingOptions.SlidingExpirationMinutes }
                }),
                new ConfigurationItem("Environment", new Dictionary<string, object> {
                    { "Mode", EnvironmentOptions.Mode },
                    { "ResetMinutes", EnvironmentOptions.ResetMinutes }
                }),
                new ConfigurationItem("Game Engine", new Dictionary<string, object> {
                    { "Game ID", GameEngineOptions.GameId },
                    { "Url", GameEngineOptions.GameEngineUrl }
                }),
                new ConfigurationItem("Domain Event Dispatcher", new Dictionary<string, object> {
                    { "Handler", DomainEventDispatcherOptions.Handler }
                })
            };

            return items.OrderBy(i => i.Name).ToList();
        }
    }
}

