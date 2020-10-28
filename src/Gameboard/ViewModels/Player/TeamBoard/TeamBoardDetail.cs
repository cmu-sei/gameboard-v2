// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;
using System;

namespace Gameboard.ViewModels
{
    public class TeamBoardDetail
    {        
        public BoardDetail Board { get; set; }

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public DateTime Start { get; set; }

        public int? OverrideMaxMinutes { get; set; }

        public int BoardMaxMinutes { get; set; }

        public double? Score { get; set; }

        public string SharedId { get; set; }
    }
}

