// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Stack.Patterns.Repository;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Repositories
{
    /// <summary>
    /// problem repository
    /// </summary>
    public class ProblemRepository : Repository<GameboardDbContext, Problem>, IProblemRepository
    {
        public ProblemRepository(GameboardDbContext db)
            : base(db) { }

        /// <summary>
        /// get all
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Problem> GetAll()
        {
            return DbContext.Problems
                .Include(p => p.Team)
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)                
                    .ThenInclude(s => s.User);
        }

        /// <summary>
        /// get all by challenge link id
        /// </summary>
        /// <param name="challengeLinkId"></param>
        /// <returns></returns>
        public IQueryable<Problem> GetAllByChallengeLinkId(string challengeLinkId)
        {
            return DbContext.Problems
                .Include(p => p.Team)
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)                
                    .ThenInclude(s => s.User)
                .Where(p => p.ChallengeLinkId == challengeLinkId);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Problem> GetById(string id)
        {
            return await DbContext.Problems
                .Include(p => p.Team)
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)
                    .ThenInclude(s => s.User)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// get by challengeLinkId and teamId
        /// </summary>
        /// <param name="challengeLinkId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<Problem> GetById(string challengeLinkId, string teamId)
        {
            return await DbContext.Problems
                .Include(p => p.Team)
                .Include(p => p.Tokens)
                .Include(p => p.Submissions)
                    .ThenInclude(s => s.User)
                .SingleOrDefaultAsync(p => p.TeamId == teamId && p.ChallengeLinkId == challengeLinkId);
        }
    }
}

