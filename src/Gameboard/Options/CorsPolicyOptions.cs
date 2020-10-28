// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Gameboard.Options
{
    /// <summary>
    /// cors policy configuration options and build method
    /// </summary>
    public class CorsPolicyOptions
    {
        public string[] Origins { get; set; }
        public string[] Methods { get; set; }
        public string[] Headers { get; set; }
        public bool AllowAnyOrigin { get; set; }
        public bool AllowAnyMethod { get; set; }
        public bool AllowAnyHeader { get; set; }
        public bool SupportsCredentials { get; set; }

        public CorsPolicy Build()
        {
            CorsPolicyBuilder policy = new CorsPolicyBuilder();
            if (AllowAnyOrigin)
                policy.AllowAnyOrigin();
            else
                policy.WithOrigins(Origins);

            if (AllowAnyHeader)
                policy.AllowAnyHeader();
            else
                policy.WithHeaders(Headers);

            if (AllowAnyMethod)
                policy.AllowAnyMethod();
            else
                policy.WithMethods(Methods);

            if (SupportsCredentials)
                policy.AllowCredentials();
            else
                policy.DisallowCredentials();

            return policy.Build();
        }
    }
}

