// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Stack.Patterns.Repository;
using System.Threading.Tasks;

namespace Gameboard.Repositories
{
    /// <summary>
    /// team board repository
    /// </summary>
    public class TeamBoardRepository : Repository<GameboardDbContext, TeamBoard>, ITeamBoardRepository
    {
        public TeamBoardRepository(GameboardDbContext db)
            : base(db)
        {
        }

        /// <summary>
        /// get by board id and team id
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<TeamBoard> GetById(string boardId, string teamId)
        {
            return await DbContext.TeamBoards
                .Include(t => t.Team)
                .SingleOrDefaultAsync(p => p.BoardId == boardId && p.TeamId == teamId);
        }
    }
}

