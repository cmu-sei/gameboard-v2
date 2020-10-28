// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Gameboard.Data;
using Gameboard.ViewModels;

namespace Gameboard.Mapping
{
    public class TokenProfile : AutoMapper.Profile
    {
        public TokenProfile()
        {
            CreateMap<Token, TokenDetail>();
        }
    }
}

