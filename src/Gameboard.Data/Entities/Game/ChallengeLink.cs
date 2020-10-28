// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gameboard.Data
{
    /// <summary>
    /// challenge link to GameEngine ChallengeSpec
    /// </summary>
    [Owned]
    public class ChallengeLink
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(40)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Slug { get; set; }
    }
}

