// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{

    public class QuestionEdit
    {
        public string Id { get; set; }
        public ChallengeLinkEdit ChallengeLink { get; set; } = new ChallengeLinkEdit();
        public int Points { get; set; }
        public int? Order { get; set; }

        public bool IsDisabled { get; set; }
    }
}

