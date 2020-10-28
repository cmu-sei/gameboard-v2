// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stack.Http.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gameboard.Api
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// add monitored scoped option
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <param name="singleton">add as a singleton to service collection</param>
        /// <returns></returns>
        public static IServiceCollection AddMonitoredOptions<TOptions>(this IServiceCollection serviceCollection, IConfiguration configuration, string key, bool singleton = false)
             where TOptions : class
        {
            var section = serviceCollection.Configure<TOptions>(configuration.GetSection(key));

            if (singleton)
            {
                return section.AddSingleton(config => config.GetService<IOptionsMonitor<TOptions>>().CurrentValue);
            }

            return section.AddScoped(config => config.GetService<IOptionsMonitor<TOptions>>().CurrentValue);
        }

        /// <summary>
        /// add default swagger configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="applicationName"></param>
        /// <param name="authorizationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string applicationName, AuthorizationOptions authorizationOptions)
        {
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new Info
                {
                    Title = applicationName,
                    Version = "v1",
                    Description = "API documentation and interaction for " + applicationName
                });

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = authorizationOptions.AuthorizationUrl,
                    Scopes = new Dictionary<string, string>
                    {
                        { authorizationOptions.AuthorizationScope, "public api access" }
                    }
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new[] { authorizationOptions.AuthorizationScope } }
                });

                c.DescribeAllEnumsAsStrings();
                c.CustomSchemaIds(x =>
                {
                    string ns = (!x.Namespace.StartsWith("Gameboard"))
                        ? x.Namespace + "."
                        : "";

                    string n = (x.IsGenericType)
                        ? x.Name.Substring(0, x.Name.Length - 2) + x.GenericTypeArguments.Last().Name.Split('.').Last()
                        : x.Name;

                    return ns + n;
                });
            });

            return services;
        }

    }
}

