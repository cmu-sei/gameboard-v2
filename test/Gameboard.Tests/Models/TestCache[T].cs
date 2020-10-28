// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Cache;
using Gameboard.Data;
using System.Collections.Generic;

namespace Gameboard.Tests
{
    public class TestCache<T> : IGameboardCache<T>
        where T : class, new()
    {
        Dictionary<string, T> _items;

        public TestCache()
        {
            _items = new Dictionary<string, T>();
        }

        public T Get(string key)
        {
            if (_items.ContainsKey(key))
                return _items[key];

            return null;
        }

        public void Remove(string key)
        {
            if (_items.ContainsKey(key))
                _items.Remove(key);

            return;
        }

        public T Set(string key, T entity)
        {
            if (_items.ContainsKey(key))
                _items.Remove(key);

            _items.Add(key, entity);

            return Get(key);
        }

        public User Set(string key, User entity)
        {
            return entity;
        }
    }
}

