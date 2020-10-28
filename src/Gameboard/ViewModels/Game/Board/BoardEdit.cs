// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class BoardEdit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public string Badges { get; set; }
        public string StartText { get; set; }
        public string RequiredBadges { get; set; }        
        public bool RequireLockedTeam { get; set; }
        public int MaxSubmissions { get; set; }
        public int MaxMinutes { get; set; }
        public List<CategoryEdit> Categories { get; set; } = new List<CategoryEdit>();        
        public List<MapEdit> Maps { get; set; } = new List<MapEdit>();
        public BoardType BoardType { get; set; }
        public bool IsPreviewAllowed { get; set; }
        public bool IsPractice { get; set; }
        public bool IsResetAllowed { get; set; }
        public bool IsTitleVisible { get; set; }
        public bool AllowSharedWorkspaces { get; set; }
        public double CertificateThreshold { get; set; }
        public int MaxConcurrentProblems { get; set; }
    }
}

