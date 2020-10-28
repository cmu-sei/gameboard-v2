// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stack.Http.Identity;
using System;

namespace Gameboard.Api.Controllers
{
    /// <summary>
    /// base api controller
    /// </summary>
    /// <remarks>manages identity and loggin</remarks>
    public abstract class ApiController : Controller
    {
        protected ILogger Logger { get; }
        protected IStackIdentityResolver IdentityResolver { get; }

        UserIdentity _identity;
        public UserIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = IdentityResolver.GetIdentityAsync().Result as UserIdentity;
                }

                return _identity;
            }
        }

        /// <summary>
        /// create an instance of controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="logger"></param>
        protected ApiController(IStackIdentityResolver identityResolver, ILogger logger)
        {
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }


}

