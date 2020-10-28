// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Api.Routines
{
    /// <summary>
    /// calculates leaderboards for games on a timer
    /// </summary>
    /// <remarks>make the second calculation configurable</remarks>
    public class LeaderboardCalculationRoutine : Routine, IRoutine
    {
        ILogger<LeaderboardCalculationRoutine> Logger { get; }

        LeaderboardOptions LeaderboardOptions { get; }

        bool _calculating;
        Timer _timer;        
        TimeSpan _timeSpan;

        public LeaderboardCalculationRoutine(IServiceProvider serviceProvider, ILogger<LeaderboardCalculationRoutine> logger, LeaderboardOptions leaderboardOptions)
            : base(serviceProvider)
        {
            LeaderboardOptions = leaderboardOptions;
            Logger = logger;
        }

        public override void Dispose()
        {
            _timer?.Dispose();            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _timeSpan = TimeSpan.FromMinutes(LeaderboardOptions.IntervalMinutes);

            _timer = new Timer(Process, null, new TimeSpan(0, 0, 30), _timeSpan);

            return Task.FromResult(0);
        }

        void Process(object state) 
        {
            if (_calculating)
            {
                Logger.LogDebug("Leaderboard Calculation Routine in progress, skipping");
            }
            else
            {
                _calculating = true;
                Logger.LogDebug("Leaderboard Calculation Routine starting");

                using (var scope = ServiceProvider.CreateScope())
                {
                    var leaderboardService = scope.ServiceProvider.GetRequiredService<LeaderboardService>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<GameboardHub, IGameboardEvent>>();

                    Logger.LogDebug("Leaderboard Calculation Routine started");

                    var leaderboards = leaderboardService.Calculate();

                    if (leaderboards.Any())
                    {
                        foreach (var leaderboard in leaderboards)
                        {
                            var gameboardEvent = hub.Clients.All; //GameboardHub.Clients.Group(id);
                            gameboardEvent.LeaderboardUpdated(leaderboard).Wait();
                        }
                    }
                    else
                    {
                        Logger.LogDebug($"Leaderboard Calculation Routine skipped, no changes");
                    }

                    _calculating = false;

                    Logger.LogDebug("Leaderboard Calculation Routine complete");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}

