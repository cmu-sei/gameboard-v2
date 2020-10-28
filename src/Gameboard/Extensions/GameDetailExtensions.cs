// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard
{
    /// <summary>
    /// game detail extensions
    /// </summary>
    public static class GameDetailExtensions
    {
        /// <summary>
        /// get all challenges for game
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static List<ChallengeDetail> GetChallenges(this GameDetail game)
        {
            return game.Boards.GetChallenges().ToList();
        }

        /// <summary>
        /// find challenge for game by challenge link id
        /// </summary>
        /// <param name="game"></param>
        /// <param name="challengeLinkId"></param>
        /// <returns></returns>
        public static ChallengeDetail FindChallengeByChallengeLinkId(this GameDetail game, string challengeLinkId)
        {            
            if (game == null)
                return null;

            return game.Boards.GetChallenges().FirstOrDefault(c => c.Id == challengeLinkId);
        }
    }
}

