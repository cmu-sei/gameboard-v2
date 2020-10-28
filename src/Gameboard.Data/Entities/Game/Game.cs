// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gameboard.Data
{
    public class Game : IAudit, ISlug
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Slug { get; set; }

        public int MaxTeamSize { get; set; }
        public int MinTeamSize { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }

        public DateTime? EnrollmentEndsAt { get; set; }

        public int MaxConcurrentProblems { get; set; }

        public ICollection<Board> Boards { get; set; } = new HashSet<Board>();

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsLocked { get; set; }
    }
}

