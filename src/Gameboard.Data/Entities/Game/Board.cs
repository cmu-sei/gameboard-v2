// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Board
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string GameId { get; set; }

        public string StartText { get; set; }

        [ForeignKey("GameId")]        
        public Game Game { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public string Badges { get; set; }
        public string RequiredBadges { get; set; }        
        public int MaxSubmissions { get; set; }
        public int MaxMinutes { get; set; }
        public bool IsPreviewAllowed { get; set; }
        public bool IsPractice { get; set; }

        public bool IsResetAllowed { get; set; }

        public bool IsTitleVisible { get; set; }

        public BoardType BoardType { get; set; } = BoardType.Trivia;

        // TRIVIA
        public ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        // MAP       
        public ICollection<Map> Maps { get; set; } = new HashSet<Map>();

        public int Order { get; set; }

        public bool AllowSharedWorkspaces { get; set; }

        public double CertificateThreshold { get; set; }

        public int MaxConcurrentProblems { get; set; }
    }

    public enum BoardType 
    { 
        Trivia,
        Map
    }
}

