// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Data.Repositories
{
    public interface ISubmissionRepository : IRepository<GameboardDbContext, Submission>
    {
        IQueryable<Submission> GetAll(string challengeId, string teamId);
        IQueryable<Submission> GetAll(string problemId);
        Task<Submission> GetById(string id);
    }
}

