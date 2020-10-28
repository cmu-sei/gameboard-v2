// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Stack.Http.Identity;
using System;
using System.Linq;

namespace Gameboard.Security
{
    /// <summary>
    /// base class to filter query and check update and delete calls based on <see cref="IStackIdentity"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class PermissionMediator<TEntity> : IPermissionMediator<TEntity>
        where TEntity : class
    {
        internal IStackIdentityResolver IdentityResolver { get; }

        IStackIdentity _identity;
        public IStackIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = IdentityResolver.GetIdentityAsync().Result;
                }

                return _identity;
            }
        }

        public UserIdentity UserIdentity => Identity as UserIdentity;
        public bool IsModerator => UserIdentity?.User?.IsModerator ?? false;
        public bool IsGameDesigner => UserIdentity?.User?.IsGameDesigner ?? false;
        public User User => UserIdentity?.User;

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        public PermissionMediator(IStackIdentityResolver identityResolver)
        {
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
        }

        public virtual IQueryable<TEntity> Process(IQueryable<TEntity> query)
        {
            return query;
        }

        public abstract bool CanUpdate(TEntity entity);

        public abstract bool CanDelete(TEntity entity);
    }
}

