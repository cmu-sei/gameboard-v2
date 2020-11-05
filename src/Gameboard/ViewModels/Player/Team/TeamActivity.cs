// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class TeamActivity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Badges { get; set; } = new List<string>();
        public bool IsDisabled { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Status { get; set; }
        public double Score { get; set; }
        public bool GamespaceReady { get; set; }
        public string ProblemId { get; set; }
        public string BoardName { get; set; }
        public string BoardId { get; set; }
        public string WorkspaceCode { get; set; }
    }

    public class TeamActivityExport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Badges { get; set; }
        public bool IsDisabled { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Status { get; set; }
        public double Score { get; set; }
        public bool GamespaceReady { get; set; }
        public string ProblemId { get; set; }
        public string BoardName { get; set; }
        public string BoardId { get; set; }
        public string WorkspaceCode { get; set; }
    }
}

