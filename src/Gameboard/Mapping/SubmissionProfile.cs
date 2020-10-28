// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Gameboard.Data;
using Gameboard.ViewModels;

namespace Gameboard.Mapping
{
    public class SubmissionProfile : AutoMapper.Profile
    {
        public SubmissionProfile()
        {
            CreateMap<Submission, SubmissionDetail>();

            CreateMap<SubmissionCreate, Submission>()
                .ForMember(d => d.Tokens,
                opt => opt.MapFrom(s => s.Tokens == null
                    ? new List<Token>()
                    : s.Tokens.Select(x => new Token { Value = x }).ToList()));

            CreateMap<Submission, GameEngine.Models.ProblemFlag>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ProblemId))
                .ForMember(d => d.SubmissionId, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Tokens, opt => opt.MapFrom(s => s.Tokens == null
                    ? new string[] { } : s.Tokens.Select(x => x.Value).ToArray()));
        }
    }
}

