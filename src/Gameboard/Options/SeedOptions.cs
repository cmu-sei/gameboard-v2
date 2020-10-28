// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gameboard
{
    /// <summary>
    /// data seeding configuration used by SeedFactory in Api
    /// </summary>
    public class SeedOptions
    {
        public string Path { get; set; }

        public bool Delete { get; set; }

        public bool OverwriteExisting { get; set; }
    }
}

