// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gameboard.Data
{
    public class Map
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ImageUrl { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string BoardId { get; set; }

        public int Order { get; set; }

        [ForeignKey("BoardId")]
        public Board Board { get; set; }

        public ICollection<Coordinate> Coordinates { get; set; } = new HashSet<Coordinate>();
    }
}

