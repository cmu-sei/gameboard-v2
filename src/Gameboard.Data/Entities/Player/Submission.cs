// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using GameEngine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Submission : ITokens
    {
        [Key]
        [MaxLength(40)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string ProblemId { get; set; }

        [ForeignKey("ProblemId")]
        public Problem Problem { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public SubmissionStatus Status { get; set; }

        /// <summary>
        /// submitted and graded tokens from <see cref="GradedSubmission.Tokens"/>
        /// </summary>
        public ICollection<Token> Tokens { get; set; } = new HashSet<Token>();
    }
}

