// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stack.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Api
{
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args)
                .ConfigureAppConfiguration((context, config) => {
                    var hostingEnvironment = context.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", false, true);
                    config.AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true, true);

                    if (hostingEnvironment.IsDevelopment())
                    {
                        config.AddJsonFile($"appsettings.{Environment.UserName}.json", true, true);
                    }

                    config.AddEnvironmentVariables();

                    if (args != null) config.AddCommandLine(args);
                })
                .Build();

            webHost.InitializeDatabase<GameboardDbContext>((db) =>
            {
                // clean up teams with no users
                var orphaned = db.Teams.Include(t => t.Users)
                    .Where(t => !t.Users.Any())
                    .ToList();

                if (orphaned.Any())
                {
                    db.Teams.RemoveRange(orphaned);
                    db.SaveChanges();
                }

                // fix invalid organization names
                var invalid = db.Users
                    .Include(u => u.Team)
                    .Where(u => u.Team != null && u.Organization != u.Team.OrganizationName)
                    .ToList();

                if (invalid.Any())
                {
                    foreach (var user in invalid)
                    {
                        user.Organization = user.Team.OrganizationName;
                    }

                    db.SaveChanges();
                }

                // add SharedIds to all TeamBoards
                var teamBoards = db.TeamBoards.Where(tb => string.IsNullOrEmpty(tb.SharedId)).ToList();

                if (teamBoards.Any())
                {
                    foreach (var teamBoard in teamBoards)
                    {
                        teamBoard.SharedId = Guid.NewGuid().ToString();
                    }

                    db.SaveChanges();
                }

                return Seed(webHost, db);
            });

            webHost.Run();
        }

        /// <summary>
        /// seed database using json files
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        static async Task Seed(IWebHost webHost, GameboardDbContext db)
        {
            try
            {
                using (var scope = webHost.Services.CreateScope())
                {
                    var hostingEnvironment = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();

                    var seedOptions = scope.ServiceProvider.GetRequiredService<SeedOptions>();

                    var factory = new SeedDataFactory(hostingEnvironment, db, seedOptions);
                    
                    var games = factory.Seed<Game>("games.json", 
                        game => db.Games
                            .Include("Boards")
                            .Include("Boards.Categories")
                            .Include("Boards.Maps")
                            .Include("Boards.Categories.Questions")
                            .Include("Boards.Maps.Coordinates")
                            .SingleOrDefault(g => g.Id.Trim().ToLower() == game.Id.Trim().ToLower()),
                        game => 
                        {
                            var remove = db.Games
                                .Include("Boards")
                                .Include("Boards.Categories")
                                .Include("Boards.Maps")
                                .Include("Boards.Categories.Questions")
                                .Include("Boards.Maps.Coordinates")
                                .SingleOrDefault(g => g.Id.Trim().ToLower() == game.Id.Trim().ToLower());

                            db.Games.Remove(remove);
                            db.SaveChanges();
                        }
                    );

                    Console.WriteLine(games.Message);

                    if (games.Exception != null)
                        Console.WriteLine(games.Exception);
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }

            return;
        }
    }
}

