// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Patterns.Repository;
using System.Threading.Tasks;

namespace Gameboard.Data.Repositories
{
    public interface ITeamRepository : IRepository<GameboardDbContext, Team>
    {
        Task<Team> GetById(string id);
    }
}

