// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Linq;
using Gameboard.Data;
using Gameboard.Identity;
using Stack.Http.Identity;

namespace Gameboard.Security
{
    public interface IPermissionMediator<TEntity> where TEntity : class
    {
        IStackIdentity Identity { get; }
        User User { get; }
        UserIdentity UserIdentity { get; }
        bool IsModerator { get; }
        bool CanDelete(TEntity entity);
        bool CanUpdate(TEntity entity);
        IQueryable<TEntity> Process(IQueryable<TEntity> query);
    }
}
