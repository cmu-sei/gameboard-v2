// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gameboard.Data
{
    public class TeamBoard
    {
        [Required(AllowEmptyStrings = false)]
        public string TeamId { get; set; }
        public Team Team { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string BoardId { get; set; }
        public DateTime Start { get; set; } = DateTime.UtcNow;
        public int? OverrideMaxMinutes { get; set; }
        public double Score { get; set; }
        public string SharedId { get; set; } = Guid.NewGuid().ToString();
    }
}

