// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameboard.Tests
{
    public class TestGameRepository : IGameRepository
    {
        public GameboardDbContext DbContext => throw new NotImplementedException();

        public async Task<Game> Add(Game entity)
        {
            return entity;
        }

        public async Task Delete(Game entity)
        {
            return;
        }

        public async Task DeleteById(int id)
        {
            return;
        }

        public async Task<bool> Exists(int id)
        {
            return false;
        }

        public IQueryable<Game> GetAll()
        {
            return new List<Game>().AsQueryable();
        }

        public async Task<Game> GetById(string id)
        {
            return null;
        }

        public async Task<Game> GetById(int id)
        {
            return null;
        }

        public async Task<Game> Update(Game entity)
        {
            return entity;
        }
    }
}

