// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Logging;
using Stack.Http.Identity;
using System;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// base api controller for services
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public abstract class ApiController<TService> : ApiController
        where TService : class
    {
        protected TService Service { get; }

        /// <summary>
        /// create an instance of controller
        /// </summary>
        /// <param name="service"></param>
        /// <param name="identityResolver"></param>
        /// <param name="logger"></param>
        protected ApiController(TService service, IStackIdentityResolver identityResolver, ILogger logger)
            : base(identityResolver, logger)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }
    }
}

