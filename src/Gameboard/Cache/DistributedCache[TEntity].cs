// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

namespace Gameboard.Cache
{
    /// <summary>
    /// caching strategy for <see cref="IDistributedCache"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DistributedCache<TEntity> : IGameboardCache<TEntity>
        where TEntity : class, new()
    {
        CachingOptions CachingOptions { get; }
        IDistributedCache Cache { get; }

        DistributedCacheEntryOptions _distributedCacheEntryOptions;

        /// <summary>
        /// options for sliding expiration
        /// </summary>
        DistributedCacheEntryOptions DistributedCacheEntryOptions
        {
            get 
            {
                if (_distributedCacheEntryOptions == null)
                {
                    _distributedCacheEntryOptions = new DistributedCacheEntryOptions();

                    if (CachingOptions.SlidingExpirationMinutes.HasValue)
                    {
                        _distributedCacheEntryOptions.SlidingExpiration = new TimeSpan(0, CachingOptions.SlidingExpirationMinutes.Value, 0);
                    }
                }

                return _distributedCacheEntryOptions;
            }
        }

        /// <summary>
        /// json serializing setting for reference loop handling
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            }
        }

        public DistributedCache(CachingOptions cachingOptions, IDistributedCache cache)
        {
            CachingOptions = cachingOptions;
            Cache = cache;
        }

        public TEntity Get(string key)
        {
            var value = Cache.GetString(key);
            if (value == null)
                return null;

            return JsonConvert.DeserializeObject<TEntity>(value);
        }

        public void Remove(string key)
        {
            if (Get(key) == null)
                return;

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

            Cache.SetString(key, JsonConvert.SerializeObject(entity, JsonSerializerSettings), DistributedCacheEntryOptions);

            return Get(key);
        }
    }
}

