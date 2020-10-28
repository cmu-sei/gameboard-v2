// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class TeamDetail : ITeamModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AnonymizedName { get; set; }
        public bool IsLocked { get; set; }
        public string OwnerUserId { get; set; }
        public List<UserDetail> Members { get; set; } = new List<UserDetail>();
        public string OrganizationLogoUrl { get; set; }
        public string OrganizationalUnitLogoUrl { get; set; }
        public string OrganizationName { get; set; }
        public int Number { get; set; }
        public DateTime Created { get; set; }
        public List<TeamBoardDetail> TeamBoards { get; set; } = new List<TeamBoardDetail>();

        public List<string> Badges { get; set; } = new List<string>();
        public bool IsDisabled { get; set; }
    }

    public interface ITeamModel
    {
        string Id { get; set; }
        string Name { get; set; }
        string AnonymizedName { get; set; }
        int Number { get; set; }
        string OrganizationLogoUrl { get; set; }
        string OrganizationalUnitLogoUrl { get; set; }
        string OrganizationName { get; set; }
    }
}

