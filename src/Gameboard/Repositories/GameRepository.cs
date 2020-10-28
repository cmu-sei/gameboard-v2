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
    /// game repository with <see cref="IPermissionMediator{TEntity}"/> support
    /// </summary>
    public class GameRepository : Repository<GameboardDbContext, Game>, IGameRepository
    {
        IPermissionMediator<Game> PermissionMediator { get; }

        public GameRepository(GameboardDbContext db, IPermissionMediator<Game> permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// get all games
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Game> GetAll()
        {
            var query = DbContext.Games
                .Include(g => g.Boards)
                .Include("Boards.Categories")
                .Include("Boards.Categories.Questions")
                .Include("Boards.Maps")
                .Include("Boards.Maps.Coordinates");

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get game by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Game> GetById(string id)
        {
            return await GetAll().SingleOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// update game which requires <see cref="User.IsGameDesigner"/> to be true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task<Game> Update(Game entity)
        {
            if (PermissionMediator.CanUpdate(entity))
                return base.Update(entity);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        /// <summary>
        /// delete game which requires <see cref="User.IsGameDesigner"/> to be true
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Task Delete(Game entity)
        {
            if (PermissionMediator.CanDelete(entity))
                return base.Delete(entity);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }
    }
}

