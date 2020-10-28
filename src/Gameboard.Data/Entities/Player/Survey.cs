// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Gameboard.Data
{
    public class Survey
    {
        public string ChallengeId { get; set; }

        public string UserId { get; set; }

        public string Data { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}

