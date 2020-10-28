// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard.Mapping
{
    public class ChallengeSpecProfile : AutoMapper.Profile
    {
        public ChallengeSpecProfile()
        {
            CreateMap<ChallengeSpec, ChallengeDetail>()
                .ForMember(d => d.Tags,
                    opt => opt.MapFrom(s => s.Tags == null
                        ? new List<string>()
                        : s.Tags.ToLower().Split(' ').ToList()))
                .ForMember(d => d.FlagCount, opt => opt.MapFrom(s => s.Flags == null ? 0 : s.Flags.Length))
                .ForMember(d => d.TokenCount, opt => opt.MapFrom(s => s.Flags == null ? 0 : s.Flags.First().Tokens.Length));

            CreateMap<ChallengeSpec, ChallengeEventDetail>()
                .ForMember(d => d.Tags,
                    opt => opt.MapFrom(s => s.Tags == null
                        ? new List<string>()
                        : s.Tags.ToLower().Split(' ').ToList()));
        }
    }
}

