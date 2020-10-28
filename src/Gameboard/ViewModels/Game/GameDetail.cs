// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class GameDetail
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public DateTime? EnrollmentEndsAt { get; set; }
        public bool IsEnrollmentAllowed 
        {
            get 
            {
                return EnrollmentEndsAt.HasValue && DateTime.UtcNow < EnrollmentEndsAt;
            }
        }
        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }
        public int MaxConcurrentProblems { get; set; }

        public bool IsMultiplayer
        {
            get { return MaxTeamSize > 1; }
        }

        public string AnonymizeTag
        {
            get 
            { 
                return IsMultiplayer ? "Team" : "Player"; 
            }
        }

        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public List<BoardDetail> Boards { get; set; }
        public bool IsLocked { get; set; }
    }
}

