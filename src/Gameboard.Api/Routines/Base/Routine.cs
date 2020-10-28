// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Api.Routines
{
    /// <summary>
    /// base routine class for default implementation
    /// </summary>
    public abstract class Routine : IRoutine
    {
        protected IServiceProvider ServiceProvider { get; }        
        public Routine(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;            
        }

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public virtual void Dispose() { }
    }
}

