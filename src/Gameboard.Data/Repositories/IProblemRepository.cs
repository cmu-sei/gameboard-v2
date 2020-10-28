// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Data.Repositories
{
    public interface IProblemRepository : IRepository<GameboardDbContext, Problem>
    {
        IQueryable<Problem> GetAllByChallengeLinkId(string challengeId);
        Task<Problem> GetById(string id);
        Task<Problem> GetById(string challengeId, string teamId);
    }
}

