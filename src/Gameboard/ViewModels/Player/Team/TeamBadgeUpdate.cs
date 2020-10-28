// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class TeamBadgeUpdate
    {
        public string Id { get; set; }
        public List<string> Badges { get; set; } = new List<string>();
    }
}

