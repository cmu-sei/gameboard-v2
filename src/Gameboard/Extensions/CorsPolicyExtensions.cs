// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Gameboard.Options;

namespace Gameboard
{
    /// <summary>
    /// cors options extensions
    /// </summary>
    public static class CorsPolicyExtensions
    {
        /// <summary>
        /// apply cors configuration to corsoptions
        /// </summary>
        /// <param name="options"></param>
        /// <param name="name"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static CorsOptions UseConfiguredCors(this CorsOptions options, string name, IConfiguration section)
        {
            CorsPolicyOptions policy = section.Get<CorsPolicyOptions>();
            options.AddPolicy(name, policy.Build());
            return options;
        }
    }

}

