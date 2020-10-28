// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.Exceptions;
using Gameboard.Services;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using GameEngine.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard
{
    public class GameFactory : IGameFactory
    {        
        IGameboardCache<Game> GameCache { get; }
        IGameboardCache<List<ChallengeSpec>> ChallengeSpecCache { get; }
        GameEngine.Client.Options GameEngineOptions { get; }
        IMapper Mapper { get; }
        IGameEngineService GameEngineService { get; }
        IServiceProvider ServiceProvider { get; }

        public GameFactory(            
            IGameboardCache<Game> gameCache,
            IGameboardCache<List<ChallengeSpec>> challengeSpecCache,
            IGameEngineService gameEngineService,
            IServiceProvider serviceProvider,
            IMapper mapper,
            GameEngine.Client.Options gameEngineOptions)
        {
            GameCache = gameCache;
            ChallengeSpecCache = challengeSpecCache;
            GameEngineService = gameEngineService;
            ServiceProvider = serviceProvider;
            Mapper = mapper;
            GameEngineOptions = gameEngineOptions;
        }

        List<ChallengeSpec> _challengeSpecs;

        List<string> GameIds { get; set; } = new List<string>();

        Game Game
        {
            get { return GetGameById(GameEngineOptions.GameId); }
        }

        /// <summary>
        /// get default game
        /// </summary>
        /// <returns></returns>
        public GameDetail GetGame()
        {
            var opts = Service.ToMappingOperations(
                new Dictionary<string, object>()
                {
                    { MappingKeys.GameEngineService, GameEngineService },
                    { MappingKeys.GameFactory, this }
                }
            );

            return Mapper.Map<GameDetail>(Game, opts);
        }

        /// <summary>
        /// get game by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Game GetGameById(string id)
        {
            var game = GameCache.Get($"{CacheKeys.Game}_{id}");

            if (game == null)
            {
                Load().Wait();
                game = GameCache.Get($"{CacheKeys.Game}_{id}");                
            }

            return game;
        }

        /// <summary>
        /// get or create challenge collection from cache
        /// </summary>
        public IEnumerable<ChallengeSpec> ChallengeSpecs
        {
            get
            {
                if (_challengeSpecs == null)
                {
                    _challengeSpecs = ChallengeSpecCache.Get(CacheKeys.ChallengeSpecs);

                    if (_challengeSpecs == null)
                    {
                        try
                        {
                            _challengeSpecs = GameEngineService.ChallengeSpecs().Result.ToList();
                        }
                        catch
                        {
                            throw new GameEngineConnectionException(GameEngineOptions.GameId, GameEngineOptions.GameEngineUrl);
                        }

                        ChallengeSpecCache.Set(CacheKeys.ChallengeSpecs, _challengeSpecs);
                    }
                }

                return _challengeSpecs;
            }
        }        

        /// <summary>
        /// generate and cache game objects for user throughout application
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            GameIds = new List<string>();

            var games = new List<Game>();

            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GameboardDbContext>();

                games = await db.Games
                    .Include(g => g.Boards)
                    .Include("Boards.Categories")
                    .Include("Boards.Categories.Questions")
                    .Include("Boards.Maps")
                    .Include("Boards.Maps.Coordinates")
                    .AsNoTracking()
                    .ToListAsync();
            }            

            foreach (var game in games)
            {
                GameIds.Add(game.Id);

                GameCache.Set($"{CacheKeys.Game}_{game.Id}", game);
            }
        }

        /// <summary>
        /// clear all challenge specs
        /// </summary>
        public void Clear()
        {
            _challengeSpecs = null;
            ChallengeSpecCache.Remove(CacheKeys.ChallengeSpecs);

            foreach (var id in GameIds)
            {
                GameCache.Remove($"{CacheKeys.Game}_{id}");
            }

            GameIds = new List<string>();
        }

        /// <summary>
        /// clear and reload games
        /// </summary>
        /// <returns></returns>
        public async Task Refresh()
        {
            Clear();
            await Load();
        }
    }
}

