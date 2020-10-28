// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System.Linq;

namespace Gameboard.Mapping
{
    public class GameProfile : AutoMapper.Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameDetail>()
                .AfterMap((s, d) =>
                {
                    d.Boards = d.Boards.OrderBy(c => c.Order).ToList();
                });

            CreateMap<GameEdit, Game>();
        }
    }
}

