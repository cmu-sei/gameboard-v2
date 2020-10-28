// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Microsoft.Extensions.Logging;
using Stack.DomainEvents;
using Stack.Http.Identity;
using Stack.Patterns.Repository;
using Stack.Validation.Handlers;
using System;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// dispatch service if inheriting servie requires domain event publishing and consumption
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DispatchService<TRepository, TEntity> : Service<TRepository, TEntity>
        where TRepository : class, IRepository<GameboardDbContext, TEntity>
        where TEntity : class, new()
    {
        public IDomainEventDispatcher DomainEventDispatcher { get; }
        ILogger<DispatchService<TRepository, TEntity>> Logger { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="logger"></param>
        /// <param name="gameFactory"></param>
        /// <param name="leaderboardOptions"></param>
        protected DispatchService(
            IDomainEventDispatcher domainEventDispatcher,
            IStackIdentityResolver identityResolver,
            TRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,
            ILogger<DispatchService<TRepository, TEntity>> logger,
            IGameFactory gameFactory,
            LeaderboardOptions leaderboardOptions)
                : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// dispatch event to publisher
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <returns></returns>
        protected async Task DispatchAsync(DomainEvent domainEvent)
        {
            var results = await DomainEventDispatcher.DispatchAsync(domainEvent);

            foreach (var result in results)
            {
                if (result.Exception == null)
                {
                    Logger.LogInformation("DomainEventDispatcher", result);
                }
                else
                {
                    Logger.LogError(result.Exception, "DomainEventDispatcher", result);
                }
            }
        }
    }
}

