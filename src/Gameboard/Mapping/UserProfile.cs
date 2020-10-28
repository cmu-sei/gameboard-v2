// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.Services;
using Gameboard.ViewModels;

namespace Gameboard.Mapping
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDetail>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team == null ? "" : src.Team.Name))
                .AfterMap((src, dest, res) =>
                {
                    var leaderboardOptions = res.GetType<LeaderboardOptions>(MappingKeys.LeaderboardOptions);
                    var userIdentity = res.GetIdentity() as UserIdentity;
                    var game = res.GetGameFactory().GetGame();

                    dest.AnonymizedTeamName = TeamExtensions.AnonymizeTeamName(src.Organization, game.AnonymizeTag, src.Team?.Number ?? 0);

                    if (src.IsAnonymizeTeam(leaderboardOptions, userIdentity))
                    {
                        dest.TeamName = dest.AnonymizedTeamName;
                    }
                });

            CreateMap<User, GameEngine.Models.Player>();
            CreateMap<UserDetail, GameEngine.Models.Player>();
        }
    }
}

