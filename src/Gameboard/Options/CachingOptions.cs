// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gameboard
{
    /// <summary>
    /// caching configuration options
    /// </summary>
    public class CachingOptions
    {
        public string CacheType { get; set; } = "Default";

        public RedisOptions Redis { get; set; }

        public int? SlidingExpirationMinutes { get; set; }
    }
}

