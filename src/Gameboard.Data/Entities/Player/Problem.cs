// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Problem : ITokens
    {
        [Key]
        [MaxLength(40)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(40)]
        public string BoardId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        public DateTime Start { get; set; } = DateTime.UtcNow;

        public DateTime? End { get; set; }

        public double Score { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// set to Game Engine Challenge.Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// set to topo mojo response text
        /// </summary>
        public string GamespaceText { get; set; }

        public int MaxSubmissions { get; set; }

        public bool HasGamespace { get; set; }

        public bool GamespaceReady { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string ChallengeLinkId { get; set; }

        /// <summary>
        /// tokens from <see cref="GameEngine.Models.ProblemState.Tokens"/>
        /// </summary>
        public ICollection<Token> Tokens { get; set; } = new HashSet<Token>();

        public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();

        public string Slug { get; set; }

        public string SharedId { get; set; }
    }
}

