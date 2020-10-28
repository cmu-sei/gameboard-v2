// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Repositories
{
    /// <summary>
    /// submission repository
    /// </summary>
    public class SubmissionRepository : Repository<GameboardDbContext, Submission>, ISubmissionRepository
    {
        public SubmissionRepository(GameboardDbContext db)
            : base(db)
        {
        }

        /// <summary>
        /// get all by challenge link id and team id
        /// </summary>
        /// <param name="challengeLinkId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public IQueryable<Submission> GetAll(string challengeLinkId, string teamId)
        {
            return DbContext.Submissions
                .Include(s => s.Tokens)
                .Include(s => s.Problem)
                .Where(s => s.Problem.ChallengeLinkId == challengeLinkId && s.Problem.TeamId == teamId);
        }

        /// <summary>
        /// get all by problem id
        /// </summary>
        /// <param name="problemId"></param>
        /// <returns></returns>
        public IQueryable<Submission> GetAll(string problemId)
        {
            return DbContext.Submissions
                .Include(s => s.Tokens)
                .Include(s => s.Problem)
                .Where(s => s.ProblemId == problemId);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Submission> GetById(string id)
        {
            return await DbContext.Submissions
                .Include(s => s.Tokens)
                .Include(s => s.Problem)
                .SingleOrDefaultAsync(s => s.Id == id);
        }
    }
}
