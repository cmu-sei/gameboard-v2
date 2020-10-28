// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System.Linq;

namespace Gameboard.Mapping
{
    public class BoardProfile : AutoMapper.Profile
    {
        public BoardProfile()
        {
            CreateMap<Board, BoardDetail>()
                .ForMember(d => d.IsActive, opt => opt.MapFrom(s => s.IsActive()))                
                .AfterMap((s, d) =>
                {
                    if (d.BoardType == BoardType.Trivia)
                    {
                        d.Categories = d.Categories.OrderBy(c => c.Order).ToList();
                    }

                    if (d.BoardType == BoardType.Map)
                    {
                        d.Maps = d.Maps.OrderBy(c => c.Order).ToList();
                    }
                });

            CreateMap<BoardEdit, Board>();
        }
    }
}

