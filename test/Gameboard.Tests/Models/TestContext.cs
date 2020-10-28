// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.Repositories;
using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Stack.DomainEvents;
using Stack.Http;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;

namespace Gameboard.Tests
{
    public class TestContext : IDisposable
    {
        readonly ILogger _logger;
        readonly IValidationHandler _validationHandler;
        TestIdentityResolver IdentityResolver { get; set; }
        User _user;

        public TestDataFactory TestDataFactory { get; set; }

        public GameboardDbContext DbContext { get; set; }
        public IGameboardCache<Game> GameCache { get; set; }

        public IGameboardCache<ChallengeSpec> ChallengeSpecCache { get; set; }

        public TestContext(
            GameboardDbContext dbContext, 
            ILogger logger, 
            IValidationHandler validationHandler, 
            IGameboardCache<Game> gameCache, 
            IGameboardCache<ChallengeSpec> challengeSpecCache)
        {
            DbContext = dbContext;
            _logger = logger;
            _validationHandler = validationHandler;
            IdentityResolver = new TestIdentityResolver(null);
            TestDataFactory = new TestDataFactory(this);
            GameCache = gameCache;
            ChallengeSpecCache = challengeSpecCache;
        }

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                IdentityResolver = new TestIdentityResolver(_user);
            }
        }

        public void Dispose() { }

        public DomainEventDispatcher GetDomainEventDispatcher()
        {
            return new DomainEventDispatcher(new DomainEventDelegator(new DomainEventDispatcherOptions(), new List<IDomainEventHandler>()));
        }

        public GameEngine.Client.Options GetGameEngineOptions()
        {
            return new GameEngine.Client.Options { GameId = "testing" };
        }

        IGameFactory _gameFactory;

        public IGameFactory GetGameFactory()
        {
            if (_gameFactory == null) {
                _gameFactory = new TestGameFactory(GameCache);
            }

            return _gameFactory;
        }

        public IValidationHandler GetValidationHandler()
        {
            return new StrictValidationHandler(
                DbContext,
                IdentityResolver,
                GameCache,
                new OrganizationOptions(),
                GetGameFactory()
            );
        }

        public IGameboardCache<T> GetCache<T>()
            where T: class, new ()
        {
            return new TestCache<T>();
        }

        public IStackIdentityResolver GetIdentityResolver()
        {
            return IdentityResolver;
        }

        public IGameRepository GetGameRepository()
        {
            return new TestGameRepository();
        }

        public IHubContext<THub, T> TestHubContext<THub, T>()
            where THub : Hub<T>
            where T : class
        {
            return new TestHubContext<THub, T>();
        }

        public IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                (typeof(Profile)).ProcessTypeOf("Gameboard", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });
            return new Mapper(configuration);
        }
    }
}
