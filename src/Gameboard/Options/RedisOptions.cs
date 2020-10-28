// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard
{
    /// <summary>
    /// redis caching configuration
    /// </summary>
    public class RedisOptions
    {
        public string Configuration { get; set; }

        public string InstanceName { get; set; }
    }
}

