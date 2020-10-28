// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.


using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class QuestionDetail : IChallengeModel
    {
        public string Id { get; set; }

        public ChallengeLinkDetail ChallengeLink { get; set; } = new ChallengeLinkDetail();

        public ChallengeDetail Challenge { get; set; }

        public int Points { get; set; }

        public int? Order { get; set; }

        public bool IsDisabled { get; set; }
    }

}

