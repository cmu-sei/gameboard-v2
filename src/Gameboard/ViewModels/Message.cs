// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{
    public class MessageCreate
    {
        public List<string> To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}

