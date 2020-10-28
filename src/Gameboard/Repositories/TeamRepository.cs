// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.Security;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Repositories
{
    /// <summary>
    /// team repository with <see cref="IPermissionMediator{TEntity}"/> support
    /// </summary>
    public class TeamRepository : Repository<GameboardDbContext, Team>, ITeamRepository
    {
        IPermissionMediator<Team> PermissionMediator { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="db"></param>
        /// <param name="permissionMediator"></param>
        public TeamRepository(GameboardDbContext db, IPermissionMediator<Team> permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// get all 
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Team> GetAll()
        {
            var query = DbContext.Teams
                .Include(x => x.Users)
                .Include(x => x.TeamBoards);

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Team> GetById(string id)
        {
            var query = DbContext.Teams
                .Include(x => x.Users)
                .Include(x => x.TeamBoards);

            return await PermissionMediator.Process(query)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// update team which requires <see cref="Team.OwnerUserId"/> matches <see cref="User.Id"/> or <see cref="User.IsModerator"/> is true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task<Team> Update(Team entity)
        {
            if (PermissionMediator.CanUpdate(entity))
                return base.Update(entity);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        /// <summary>
        /// delete team which requires that <see cref="User.IsModerator"/> is true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task Delete(Team entity)
        {
            if (PermissionMediator.CanDelete(entity))
                return base.Delete(entity);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }
    }
}

