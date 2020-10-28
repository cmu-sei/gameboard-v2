// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace Gameboard.Mapping
{
    public class TeamProfile : AutoMapper.Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamDetail>()
                .ForMember(d => d.Members, opt => opt.MapFrom(s => s.Users))
                .ForMember(d => d.TeamBoards, opt => opt.MapFrom(s => s.TeamBoards))
                .ForMember(d => d.Badges, opt => opt.MapFrom(s => s.Badges.ToBadgeArray()))
                .AfterMap((src, dest, res) =>
                {
                    var leaderboardOptions = res.GetType<LeaderboardOptions>(MappingKeys.LeaderboardOptions);
                    var userIdentity = res.GetIdentity() as UserIdentity;
                    var game = res.GetGameFactory().GetGame();

                    dest.AnonymizedName = dest.AnonymizeTeamName(game);

                    if (dest.IsAnonymizeTeam(leaderboardOptions, userIdentity))
                    {
                        dest.Name = dest.AnonymizedName;
                    }

                    if (userIdentity == null ||
                        (userIdentity != null && userIdentity.User.TeamId != dest.Id && !userIdentity.User.IsModerator))
                    {
                        dest.Members.Clear();
                        //dest.OwnerUserId = null;
                    }

                    if (dest.TeamBoards.Any())                    
                    {
                        dest.TeamBoards = dest.TeamBoards.OrderBy(t => t.Board?.Order ?? 0).ToList();
                    }
                });

            CreateMap<Team, TeamSummary>()
                .ForMember(d => d.Badges, opt => opt.MapFrom(s => s.Badges.ToBadgeArray()))
                .AfterMap((src, dest, res) =>
                {
                    var userIdentity = res.GetIdentity() as UserIdentity;
                });

            CreateMap<TeamActivity, TeamActivityExport>()
                .ForMember(d => d.Badges, opt => opt.MapFrom(s => string.Join(" ", s.Badges)));

            CreateMap<Team, GameEngine.Models.PlayerTeam>()
                .ForMember(d => d.Players, opt => opt.MapFrom(s => s.Users));
            CreateMap<TeamDetail, GameEngine.Models.PlayerTeam>()
                .ForMember(d => d.Players, opt => opt.MapFrom(s => s.Members));
        }
    }
}

