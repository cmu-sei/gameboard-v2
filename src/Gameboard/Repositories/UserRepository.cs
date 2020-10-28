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
    /// user repository with <see cref="IPermissionMediator{TEntity}"/> support
    /// </summary>
    public class UserRepository : Repository<GameboardDbContext, User>, IUserRepository
    {
        IPermissionMediator<User> PermissionMediator { get; }

        public UserRepository(GameboardDbContext db, IPermissionMediator<User> permissionMediator)
            : base(db) 
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// update user if <see cref="User.Id"/> matches model or <see cref="User.IsModerator"/> is true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task<User> Update(User entity)
        {
            if (PermissionMediator.CanUpdate(entity))
                return base.Update(entity);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        /// <summary>
        /// get all
        /// </summary>
        /// <returns></returns>
        public override IQueryable<User> GetAll()
        {
            return base.GetAll()
                .Include(u => u.Team);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetById(string id)
        {
            return await DbContext.Users
                .Include(x => x.Team)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// delete user if <see cref="User.IsModerator"/> is true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(User entity)
        {
            if (PermissionMediator.CanDelete(entity))
                await base.Delete(entity);
        }
    }
}

