// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.


using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public interface IChallengeModel
    {
        ChallengeLinkDetail ChallengeLink { get; set; }

        /// <summary>
        /// mapped by GameFactory
        /// </summary>
        ChallengeDetail Challenge { get; set; }

        int Points { get; set; }
    }
}

