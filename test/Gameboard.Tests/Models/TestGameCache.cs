// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using System;

namespace Gameboard.Tests
{
    public class TestChallengeSpecCache : IGameboardCache<ChallengeSpec>
    {
        public ChallengeSpec Get(string key)
        {
            return null;
        }

        public void Remove(string key)
        {
            return;
        }

        public ChallengeSpec Set(string key, ChallengeSpec entity)
        {
            return entity;
        }
    }

    public class TestGameCache : IGameboardCache<Game>
    {
        public TestGameCache()
        {
            
        }

        Game Game { get; set; }

        public Game Get(string key)
        {
            if (Game != null && Game.Id == key)
                return Game;

            return null;
        }

        public void Remove(string key)
        {
            return;
        }

        public Game Set(string key, Game item)
        {
            Game = item;
            return Game;
        }
    }
}

