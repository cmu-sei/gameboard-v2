// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Api.Routines;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.HostedServices
{
    /// <summary>
    /// hosted service to run and manage routines
    /// </summary>
    /// <remarks>processes routines</remarks>
    public class RoutineHostedService : HostedService, IHostedService, IDisposable
    {
        List<IRoutine> Routines { get; } = new List<IRoutine>();        
        
        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="routines">IRoutine collection defined in <see cref="Api.Startup"/></param>
        public RoutineHostedService(
            IServiceProvider serviceProvider,
            IEnumerable<IRoutine> routines)
            : base(serviceProvider)
        {            
            Routines = routines.ToList();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var routine in Routines)
                await routine.StartAsync(cancellationToken);

            return;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var routine in Routines)
                await routine.StopAsync(cancellationToken);

            return;
        }

        public void Dispose()
        {
            foreach (var routine in Routines)
                routine.Dispose();
        }
    }
}

