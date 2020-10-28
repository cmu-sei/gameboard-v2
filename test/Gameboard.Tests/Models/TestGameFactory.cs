// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Tests
{
    public class TestGameFactory : IGameFactory
    {
        IGameboardCache<Game> GameCache { get; }

        public TestGameFactory(IGameboardCache<Game> gameCache)
        {
            GameCache = gameCache;
        }

        public IEnumerable<ChallengeDetail> Challenges => new List<ChallengeDetail>();

        public IEnumerable<ChallengeSpec> ChallengeSpecs => new List<ChallengeSpec>();
        
        public void Clear()
        {
            GameCache.Remove("testing");
        }

        DateTime? DefaultEnrollmentEnds { get; set; } = DateTime.UtcNow.AddDays(10);
        int DefaultMinTeamSize { get; set; } = 1;
        int DefaultMaxTeamSize { get; set; } = 5;

        public async Task Load()
        {
            _game = new GameDetail
            {
                EnrollmentEndsAt = DefaultEnrollmentEnds,
                MaxTeamSize = DefaultMaxTeamSize,
                MinTeamSize = DefaultMinTeamSize,
                Name = "Testing",
                Id = "testing",
                Boards = new List<BoardDetail>
                {
                    new BoardDetail { Id = "round-1", Name = "Round 1" },
                    new BoardDetail { Id = "round-2", Name = "Round 2" }
                }
            };

            GameCache.Set("testing", null);
        }

        public async Task Refresh()
        {
            Clear();

            await Load();
        }

        GameDetail _game;

        public GameDetail Game 
        { 
            get 
            {
                if (_game == null)
                {
                    Load().Wait();
                }

                return _game;
            }
            set 
            {
                _game = value;
            }
        }

        public GameDetail GetGame()
        {
            return Game;
        }
    }
}

