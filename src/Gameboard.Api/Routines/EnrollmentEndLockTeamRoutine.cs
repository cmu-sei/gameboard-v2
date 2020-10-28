// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Hubs;
using Gameboard.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Api.Routines
{
    /// <summary>
    /// routine to lock teams that are unlocked when enrollment ends
    /// </summary>
    public class EnrollmentEndLockTeamRoutine : Routine, IRoutine
    {
        Timer _timer;
        ILogger<EnrollmentEndLockTeamRoutine> Logger { get; }
        IGameFactory GameFactory { get; }
        IHubContext<GameboardHub, IGameboardEvent> GameboardHub { get; }

        bool _locking = false;

        public EnrollmentEndLockTeamRoutine(
            IServiceProvider serviceProvider, 
            ILogger<EnrollmentEndLockTeamRoutine> logger, 
            IGameFactory gameFactory,
            IHubContext<GameboardHub, IGameboardEvent> gameboardHub)
            : base(serviceProvider)
        {
            Logger = logger;
            GameFactory = gameFactory;
            GameboardHub = gameboardHub;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Process, null, new TimeSpan(0, 0, 0), new TimeSpan(0, 5, 0));

            return Task.FromResult(0);
        }

        void Process(object state)
        {
            if (_locking)
            {
                Logger.LogDebug($"Lock Teams Routine already running");
            }
            else
            {
                Logger.LogDebug($"Lock Teams Routine started");

                _locking = true;

                try
                {
                    var game = GameFactory.GetGame();

                    if (game.EnrollmentEndsAt.HasValue && game.EnrollmentEndsAt.Value < DateTime.UtcNow && !game.IsLocked)
                    {
                        Logger.LogDebug($"Lock Teams Routine locked at {0}", game.EnrollmentEndsAt);
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var gameService = scope.ServiceProvider.GetRequiredService<GameService>();
                            var teamService = scope.ServiceProvider.GetRequiredService<TeamService>();

                            var db = teamService.Repository.DbContext;

                            // get all teams where team is not locked
                            var teams = db.Teams
                                .Include(t => t.Users)
                                .Where(t => !t.IsLocked);

                            if (teams.Any())
                            {
                                // remove teams with no organization
                                var removeTeams = teams.Where(t => string.IsNullOrWhiteSpace(t.OrganizationName)).ToList();

                                // if game has a min team size remove teams below threshold
                                if (game.MinTeamSize > 0)
                                {
                                    removeTeams.AddRange(teams.Where(t => t.Users.Count() < game.MinTeamSize));
                                    removeTeams = removeTeams.Distinct().ToList();
                                }

                                var removeTeamsCount = removeTeams.Count();

                                // lock teams not about to be removed
                                var lockTeams = teams.Except(removeTeams).ToList();
                                var lockedTeamsCount = lockTeams.Count();

                                // lock each unlocked team
                                foreach (var team in lockTeams)
                                {
                                    team.IsLocked = true;
                                
                                    db.SaveChanges();
                                    teamService.RemoveUsersFromCache(team);
                                    var model = teamService.GetById(team.Id).Result;
                                    GameboardHub.Clients.Group(model.Id).TeamUpdated(model).Wait();
                                }

                                if (removeTeams.Any()) 
                                {
                                    db.Teams.RemoveRange(removeTeams);
                                    db.SaveChanges();
                                }

                                Logger.LogDebug($"Lock Teams Routine completed, {lockedTeamsCount} teams locked, {removeTeamsCount} teams removed.");
                            }
                            else 
                            {
                                Logger.LogDebug($"Lock Teams Routine skipped, no teams to lock");
                            }

                            // set IsLocked to prevent future locking to take place
                            db.Games.Single(g => g.Id == game.Id).IsLocked = true;
                            db.SaveChanges();

                            GameFactory.Refresh();
                            game = GameFactory.GetGame();
                            GameboardHub.Clients.All.GameUpdated(game).Wait();                            
                        }
                    }
                    else
                    {
                        Logger.LogDebug($"Lock Teams Routine skipped, not game to lock");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, $"Lock Teams Routine failed");
                }
                finally
                {
                    _locking = false;
                }
                
            }            
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

