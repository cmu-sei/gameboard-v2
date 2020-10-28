// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System.Linq;

namespace Gameboard.Mapping
{
    public class CoordinateProfile : AutoMapper.Profile
    {
        public CoordinateProfile()
        {
            CreateMap<Coordinate, CoordinateDetail>()
            .AfterMap((src, dest, res) =>
            {
                var gameFactory = res.GetGameFactory();

                if (gameFactory != null) 
                {
                    var spec = gameFactory.ChallengeSpecs.FirstOrDefault(s => s.Slug == src.ChallengeLink.Slug);

                    if (spec != null)
                    {
                        dest.Challenge = res.Mapper.Map<ChallengeDetail>(spec);
                        dest.Challenge.Id = src.ChallengeLink.Id;
                        dest.Challenge.Points = src.Points;
                    }
                }
            });

            CreateMap<CoordinateEdit, Coordinate>();
        }
    }
}

