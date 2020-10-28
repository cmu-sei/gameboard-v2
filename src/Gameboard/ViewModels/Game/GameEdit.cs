// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class GameEdit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? EnrollmentEndsAt { get; set; }
        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }
        public int MaxConcurrentProblems { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public List<BoardEdit> Boards { get; set; } = new List<BoardEdit>();
    }
}

