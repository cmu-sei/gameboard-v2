// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Caching.Memory;
using System;

namespace Gameboard.Cache
{
    /// <summary>
    /// caching strategy for <see cref="IMemoryCache"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MemoryCache<TEntity> : IGameboardCache<TEntity>
        where TEntity : class, new()
    {
        CachingOptions CachingOptions { get; }
        IMemoryCache Cache { get; }

        MemoryCacheEntryOptions _memoryCacheEntryOptions;

        /// <summary>
        /// options for sliding expiration
        /// </summary>
        MemoryCacheEntryOptions MemoryCacheEntryOptions
        {
            get 
            {
                if (_memoryCacheEntryOptions == null)
                {
                    _memoryCacheEntryOptions = new MemoryCacheEntryOptions();

                    if (CachingOptions.SlidingExpirationMinutes.HasValue)
                    {
                        _memoryCacheEntryOptions.SlidingExpiration = new TimeSpan(0, CachingOptions.SlidingExpirationMinutes.Value, 0);
                    }
                }

                return _memoryCacheEntryOptions;
            }
        }

        public MemoryCache(CachingOptions cachingOptions, IMemoryCache cache)
        {
            CachingOptions = cachingOptions;
            Cache = cache;
        }

        public TEntity Get(string key)
        {
            if (Cache.TryGetValue(key, out TEntity entity))
                return entity;

            return null;
        }

        public void Remove(string key)
        {
            if (Cache.TryGetValue(key, out TEntity entity))
                Cache.Remove(key);
        }

        public TEntity Set(string key, TEntity entity)
        {
            if (entity == null)
            {
                if (Get(key) != null)
                    Remove(key);

                return null;
            }

            return Cache.Set(key, entity, MemoryCacheEntryOptions);
        }
    }
}

