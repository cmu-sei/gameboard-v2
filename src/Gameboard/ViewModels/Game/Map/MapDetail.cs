// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.


using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class MapDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string BoardId { get; set; }
        public int Order { get; set; }

        public List<CoordinateDetail> Coordinates { get; set; } = new List<CoordinateDetail>();
    }
}

