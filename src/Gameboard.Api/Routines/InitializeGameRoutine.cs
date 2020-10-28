// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Api.Routines
{
    /// <summary>
    /// refreshes initial game state
    /// </summary>
    public class InitializeGameRoutine : Routine, IRoutine
    {
        ILogger<InitializeGameRoutine> Logger { get; }
        IGameFactory GameFactory { get; }

        public InitializeGameRoutine(IServiceProvider serviceProvider, ILogger<InitializeGameRoutine> logger, IGameFactory gameFactory)
            : base(serviceProvider)
        {
            Logger = logger;
            GameFactory = gameFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Process(cancellationToken);

            return Task.CompletedTask;
        }

        void Process(object state) 
        {
            try
            {
                Logger.LogDebug($"Initialize Game Routine started");
                GameFactory.Refresh().Wait();
                Logger.LogDebug($"Initialize Game Routine finished");
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, $"Initialize Game Routine failed");
            }
        }
    }
}

