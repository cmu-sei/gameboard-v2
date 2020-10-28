// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Patterns.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gameboard.Data.Repositories
{
    public interface ITeamBoardRepository : IRepository<GameboardDbContext, TeamBoard>
    {
        Task<TeamBoard> GetById(string boardId, string teamId);
    }
}

