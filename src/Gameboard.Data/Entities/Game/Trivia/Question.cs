// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Question
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CategoryId { get; set; }

        public ChallengeLink ChallengeLink { get; set; } = new ChallengeLink();

        public int Points { get; set; }

        public int? Order { get; set; }

        public bool IsDisabled { get; set; }
    }
}

