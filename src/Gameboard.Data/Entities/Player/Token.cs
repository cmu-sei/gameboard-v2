// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Token
    {
        [Key]
        [MaxLength(40)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Value { get; set; }
        public int Percent { get; set; }
        public TokenStatusType Status { get; set; }
        public DateTime? Timestamp { get; set; } = DateTime.UtcNow;

        public int? Index { get; set; }

        public string Label { get; set; }

        public string SubmissionId { get; set; }

        [ForeignKey("SubmissionId")]
        public Submission Submission { get; set; }

        public string ProblemId { get; set; }

        [ForeignKey("ProblemId")]
        public Problem Problem { get; set; }
    }
}

