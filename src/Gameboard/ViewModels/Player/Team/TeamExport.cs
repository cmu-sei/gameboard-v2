// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class TeamExport
    {
        public string Name { get; set; }

        public string Anonymized { get; set; }

        public bool Locked { get; set; }

        public string Organization { get; set; }

        public string Created { get; set; }

        public string Members { get; set; }

        public string Status { get; set; }
    }
}

