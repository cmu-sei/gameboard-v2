// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class TeamSummary
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<string> Badges { get; set; } = new List<string>();

        public bool IsDisabled { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationLogoUrl { get; set; }

    }
}

