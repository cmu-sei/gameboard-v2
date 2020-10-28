// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Coordinate
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        [Required]
        public double X { get; set; }

        [Required]
        public double Y { get; set; }

        [Required]
        public double Radius { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MapId { get; set; }

        [ForeignKey("MapId")]
        public Map Map { get; set; }

        public int Points { get; set; }

        public ActionType ActionType { get; set; }

        public string ActionValue { get; set; }

        public ChallengeLink ChallengeLink { get; set; } = new ChallengeLink();

        public bool IsDisabled { get; set; }

    }

    public enum ActionType
    { 
        Challenge,
        Map,
        Video,
        Image,
        Document
    }
}

