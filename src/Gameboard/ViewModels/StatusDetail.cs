// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gameboard.ViewModels
{
    public class StatusDetail
    {
        public string Commit { get; set; }
        public ApiStatus Status { get; set; }
    }
}

