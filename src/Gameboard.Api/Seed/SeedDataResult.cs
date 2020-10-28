// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard.Api
{
    /// <summary>
    /// seed data result for logging
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SeedDataResult<TEntity>
        where TEntity : class, new()
    {
        public List<SeedDataResultItem<TEntity>> Items { get; set; } = new List<SeedDataResultItem<TEntity>>();

        public List<TEntity> Entities { get; set; } = new List<TEntity>();

        public int SuccessCount { get { return Items.Count(i => i.Status == SeedDataResultItemStatusType.Success); } }

        public int ExistsCount { get { return Items.Count(i => i.Status == SeedDataResultItemStatusType.Exists); } }

        public Exception Exception { get; set; }

        public string Message { get; set; }
    }

    public class SeedDataResultItem<TEntity>
        where TEntity : class, new()
    {
        public TEntity Entity { get; set; }

        public SeedDataResultItemStatusType Status { get; set; }
    }

    public enum SeedDataResultItemStatusType
    {
        NotSet = 0,
        Success = 1,
        Exists = 2
    }
}

