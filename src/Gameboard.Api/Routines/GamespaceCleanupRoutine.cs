// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Api.Routines
{
    /// <summary>
    /// cleans up expired gamespaces
    /// </summary>
    public class GamespaceCleanupRoutine : Routine, IRoutine
    {
        Timer _timer;
        ILogger<GamespaceCleanupRoutine> Logger { get; }

        public GamespaceCleanupRoutine(IServiceProvider serviceProvider, ILogger<GamespaceCleanupRoutine> logger)
            : base(serviceProvider)
        {
            Logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: randomize timer and implement lock for multi-replica deployments
            _timer = new Timer(Process, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 5, 0));

            return Task.FromResult(0);
        }

        void Process(object state)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var problemService = scope.ServiceProvider.GetRequiredService<ProblemService>();
                problemService.ExpireGamespaces().Wait();
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

