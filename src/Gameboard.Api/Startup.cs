// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Api.Routines;
using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.HostedServices;
using Gameboard.Hubs;
using Gameboard.Identity;
using Gameboard.Integrations;
using Gameboard.Options;
using Gameboard.Repositories;
using Gameboard.Security;
using Gameboard.Services;
using GameEngine.Abstractions;
using GameEngine.Abstractions.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stack.Data;
using Stack.Data.Options;
using Stack.DomainEvents;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gameboard.Api
{
    public class Startup
    {
        string ApplicationName { get; set; } = "";
        public CachingOptions CachingOptions { get; set; } = new CachingOptions();

        public Stack.Http.Options.AuthorizationOptions AuthorizationOptions { get; set; } = new Stack.Http.Options.AuthorizationOptions();
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection("Authorization").Bind(AuthorizationOptions);
            Configuration.GetSection("Caching").Bind(CachingOptions);
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        /// <summary>
        /// the supported services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDataProtection().SetApplicationName($"dp:{Assembly.GetEntryAssembly().FullName}");

            services.AddDbProvider(Configuration);
            services.AddDbContextPool<GameboardDbContext>(builder => builder.UseConfiguredDatabase("Gameboard.Data", Configuration));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwagger(ApplicationName, AuthorizationOptions);

            // configure caching strategy
            // TODO: handle scenario where no caching is required with a basic handler
            if (CachingOptions.CacheType == CacheType.Redis)
            {
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = CachingOptions.Redis.Configuration;
                    options.InstanceName = CachingOptions.Redis.InstanceName;
                });

                services.AddSingleton<IGameboardCache<User>, DistributedCache<User>>();
                services.AddSingleton<IGameboardCache<Leaderboard>, DistributedCache<Leaderboard>>();
                services.AddSingleton<IGameboardCache<Game>, DistributedCache<Game>>();
                services.AddSingleton<IGameboardCache<List<ChallengeSpec>>, DistributedCache<List<ChallengeSpec>>>();
            }
            else
            {
                services.AddMemoryCache();

                services.AddSingleton<IGameboardCache<User>, MemoryCache<User>>();
                services.AddSingleton<IGameboardCache<Leaderboard>, MemoryCache<Leaderboard>>();
                services.AddSingleton<IGameboardCache<Game>, MemoryCache<Game>>();
                services.AddSingleton<IGameboardCache<List<ChallengeSpec>>, MemoryCache<List<ChallengeSpec>>>();
            }

            services.AddLogging();

            services
                .AddMonitoredOptions<BrandingOptions>(Configuration, "Branding")
                .AddMonitoredOptions<DatabaseOptions>(Configuration, "Database")
                .AddMonitoredOptions<ErrorHandlingOptions>(Configuration, "ErrorHandling")
                .AddMonitoredOptions<OrganizationOptions>(Configuration, "Options:Organization")
                .AddMonitoredOptions<EnvironmentOptions>(Configuration, "Options:Environment", true)
                .AddMonitoredOptions<Stack.Http.Options.AuthorizationOptions>(Configuration, "Authorization", true)
                .AddMonitoredOptions<MailOptions>(Configuration, "Mail", true)
                .AddMonitoredOptions<SeedOptions>(Configuration, "Options:Seed", true)
                .AddMonitoredOptions<RedisOptions>(Configuration, "Options:Redis", true)
                .AddMonitoredOptions<LeaderboardOptions>(Configuration, "Options:Leaderboard", true)
                .AddMonitoredOptions<CachingOptions>(Configuration, "Caching", true)
                .AddMonitoredOptions<DomainEventDispatcherOptions>(Configuration, "Options:DomainEventDispatcher", true);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // identity
            services.AddScoped<IStackIdentityResolver, UserIdentityResolver>();
            services.AddScoped<IValidationHandler, ServiceProviderValidationHandler>();

            // services
            services.AddScoped<TeamService>();
            services.AddScoped<UserService>();
            services.AddScoped<BoardService>();
            services.AddScoped<ProblemService>();
            services.AddScoped<SubmissionService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<LeaderboardService>();
            services.AddScoped<TeamBoardService>();
            services.AddScoped<GameService>();

            // permission mediators
            // these are embeded in data repositories for identity permission checks
            // on query execution
            services.AddScoped<IPermissionMediator<Team>, TeamPermissionMediator>();
            services.AddScoped<IPermissionMediator<Game>, GamePermissionMediator>();
            services.AddScoped<IPermissionMediator<User>, UserPermissionMediator>();

            // repositories
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<ISubmissionRepository, SubmissionRepository>();
            services.AddScoped<ITeamBoardRepository, TeamBoardRepository>();

            // main game/board/challenge interface for game data fetching
            services.AddSingleton<IGameFactory, GameFactory>();

            services.AddGameEngineClient(() =>
                Configuration.GetSection("Options:GameEngine").Get<GameEngine.Client.Options>()
            );
            services.AddScoped<IGameEngineEventHandler, GameEngineEventHandler>();

            // single hosted service that manages defined routines                        
            services.AddHostedService<RoutineHostedService>();

            // hosted service routines called by RoutingHostedService
            services.AddSingleton<IRoutine, InitializeGameRoutine>();
            services.AddSingleton<IRoutine, LeaderboardCalculationRoutine>();
            services.AddSingleton<IRoutine, GamespaceCleanupRoutine>();
            services.AddSingleton<IRoutine, EnrollmentEndLockTeamRoutine>();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddResponseCompression();
            services.AddResponseCaching(opt => { });

            services.Configure<SecurityHeaderOptions>(Configuration.GetSection("SecurityHeaders"))
                .AddScoped(svc => svc.GetService<IOptionsSnapshot<SecurityHeaderOptions>>().Value);

            services.AddCors(options => options.UseConfiguredCors("default", Configuration.GetSection("CorsPolicy")));

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddMvcCore(options => options.InputFormatters.Insert(0, new TextMediaTypeFormatter()))
                .AddApiExplorer()
                .AddJsonFormatters();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthorizationOptions.Authority;
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                })
                .AddTicketAuthentication(TicketAuthentication.AuthenticationScheme, options => {});

            services.AddAuthorization(_ =>
            {
                _.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme
                    ).Build();

                _.AddPolicy("OneTimeTicket", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(TicketAuthentication.AuthenticationScheme)
                    .Build());
            });

            ISignalRServerBuilder signalR = services.AddSignalR();
            services.AddSingleton<IUserIdProvider, HubSubjectProvider>();

            if (CachingOptions.CacheType == CacheType.Redis)
            {
                signalR.AddStackExchangeRedis(CachingOptions.Redis.Configuration,
                    options => options.Configuration.ChannelPrefix = ApplicationName);
            }

            signalR.AddJsonProtocol(options => options.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            bool showDeveloperExceptions = env.IsDevelopment() || Configuration.GetValue("ErrorHandling:ShowDeveloperExceptions", false);

            if (showDeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseCors("default");
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/{documentName}/api.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/api/v1/api.json", ApplicationName + " (v1)");
                c.OAuthClientId(AuthorizationOptions.ClientId);
                c.OAuthClientSecret(AuthorizationOptions.ClientSecret);
                c.OAuthAppName(AuthorizationOptions.ClientName);
            });

            app.UseSignalR(config => config.MapHub<GameboardHub>("/hub"));

            app.UseGameEngineCallback();
            app.UseMvcWithDefaultRoute();

            app.Use(async (context, next) =>
            {
                await next();
            });
        }
    }
}

