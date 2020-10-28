// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Caching;

namespace Gameboard.Cache
{
    public interface IGameboardCache<TEntity> : IEntityCache<TEntity>
        where TEntity : class, new()
    {
    }
}

