// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;

namespace Gameboard.Mapping
{
    public class LeaderboardProfile : AutoMapper.Profile
    {
        public LeaderboardProfile()
        {
            // TODO: I think this is not necessary
            CreateMap<Leaderboard, Leaderboard>();
        }
    }
}

