// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System.Linq;

namespace Gameboard.Mapping
{
    public class ChallengeLinkProfile : AutoMapper.Profile
    {
        public ChallengeLinkProfile()
        {
            CreateMap<ChallengeLink, ChallengeLinkDetail>();
            CreateMap<ChallengeLinkEdit, ChallengeLink>();
        }
    }
}

