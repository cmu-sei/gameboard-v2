// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System;
using System.Linq;

namespace Gameboard.Mapping
{
    public class ProblemProfile : AutoMapper.Profile
    {
        public ProblemProfile()
        {
            CreateMap<Problem, ProblemDetail>()
                .AfterMap((s, d) =>
                {
                    d.Tokens = d.Tokens.OrderBy(t => t.Index).ToArray();
                    d.Submissions = d.Submissions.OrderBy(t => t.Timestamp).ToArray();
                    d.Submissions.ToList().ForEach(x => x.Tokens = x.Tokens.OrderBy(t => t.Index).ToList());
                });

            CreateMap<Problem, GamespaceDetail>()
                .AfterMap((src, dest, res) =>
                {
                    var gameFactory = res.GetGameFactory();
                    var game = gameFactory?.GetGame();
                    var board = game.Boards.SingleOrDefault(b => b.Id == src.BoardId);

                    IChallengeModel challenge = null;

                    if (board?.BoardType == BoardType.Map)
                        challenge = board.Maps.SelectMany(c => c.Coordinates).SingleOrDefault(c => c.ChallengeLink.Id == src.ChallengeLinkId);

                    if (board?.BoardType == BoardType.Trivia)
                        challenge = board.Categories.SelectMany(c => c.Questions).SingleOrDefault(c => c.ChallengeLink.Id == src.ChallengeLinkId);

                    if (challenge != null)
                    {
                        dest.ChallengeId = challenge.ChallengeLink.Id;
                        dest.ChallengeSlug = challenge.ChallengeLink.Slug;
                        dest.ChallengeTitle = challenge.Challenge.Title;
                        dest.ChallengePoints = challenge.Points;
                    }
                });

            CreateMap<ChallengeStart, Problem>();

            CreateMap<Problem, GameEngine.Models.Problem>();

            CreateMap<ChallengeLink, GameEngine.Models.ChallengeLink>();

            CreateMap<GameEngine.Models.ConsoleSummary, ProblemConsoleSummary>();
            CreateMap<ProblemConsoleAction, GameEngine.Models.VmAction>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.VmId));
        }
    }
}

