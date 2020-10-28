// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gameboard.Data
{
    interface ISlug
    {
        string Name { get; set; }

        string Slug { get; set; }
    }
}

