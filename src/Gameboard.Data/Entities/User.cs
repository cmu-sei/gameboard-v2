// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class User
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Organization { get; set; }

        [MaxLength(40)]
        public string TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        /// <summary>
        /// manage users, manage teams, view game stats
        /// </summary>
        public bool IsModerator { get; set; }

        /// <summary>
        /// view game stats
        /// </summary>
        public bool IsObserver { get; set; }

        /// <summary>
        /// view and manage challenges
        /// </summary>
        public bool IsChallengeDeveloper { get; set; }

        /// <summary>
        /// create and manage games
        /// </summary>
        public bool IsGameDesigner { get; set; }

        public string Survey { get; set; }
    }
}

