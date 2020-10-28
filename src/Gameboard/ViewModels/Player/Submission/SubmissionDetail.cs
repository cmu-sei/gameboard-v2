// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class SubmissionDetail
    {
        public string Id { get; set; }

        public string ProblemId { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime Timestamp { get; set; }

        public string Status { get; set; }

        public List<TokenDetail> Tokens { get; set; } = new List<TokenDetail>();
    }

    public class TokenDetail
    {
        public string Id { get; set; }
        public int Percent { get; set; }
        public int? Index { get; set; }
        public string Label { get; set; }
        public TokenStatusType Status { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}

