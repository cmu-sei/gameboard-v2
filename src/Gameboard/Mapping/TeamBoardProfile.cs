// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Gameboard.Data;
using Gameboard.ViewModels;
using GameEngine.Client.Extensions;

namespace Gameboard.Mapping
{
    public class TeamBoardProfile : AutoMapper.Profile
    {
        public TeamBoardProfile()
        {
            CreateMap<TeamBoard, TeamBoardDetail>()
                .ForMember(d => d.TeamName, opt => opt.MapFrom(s => s.Team.Name))
                .AfterMap((src, dest, res) =>
                {
                    var gameFactory = res.GetGameFactory();

                    if (gameFactory != null)
                    {
                        var game = gameFactory.GetGame();
                        
                        if (game == null)
                            return;

                        dest.Board = game.Boards.FirstOrDefault(b => b.Id == src.BoardId);                        
                    }
                });

            CreateMap<TeamBoard, TeamBoardEventDetail>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Team.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Team.Name))
                .AfterMap((src, dest, res) =>
                {
                    var gameFactory = res.GetGameFactory();

                    if (gameFactory != null)
                    {
                        var game = gameFactory.GetGame();
                        var board = game.Boards.FirstOrDefault(b => b.Id == src.BoardId);

                        if (board == null)
                            return;

                        dest.Board = board;

                        var challenges = new List<ChallengeEventDetail>();

                        if (board.BoardType == BoardType.Trivia)
                        {
                            foreach (var category in board.Categories)
                            {
                                foreach (var question in category.Questions)
                                {
                                    if (question.Challenge != null)
                                    {
                                        challenges.Add(new ChallengeEventDetail
                                        {
                                            Name = category.Name,
                                            Id = question.Challenge.Id,
                                            Points = question.Points,
                                            Title = question.Challenge.Title
                                        });
                                    }
                                }
                            }
                        }

                        if (board.BoardType == BoardType.Map)
                        {
                            foreach (var map in board.Maps)
                            {
                                foreach (var coordinate in map.Coordinates)
                                {
                                    if (coordinate.ActionType == ActionType.Challenge && coordinate.Challenge != null)
                                    {
                                        challenges.Add(new ChallengeEventDetail
                                        {
                                            Name = map.Name,
                                            Id = coordinate.Challenge.Id,
                                            Points = coordinate.Points,
                                            Title = coordinate.Challenge.Title
                                        });
                                    }
                                }
                            }
                        }

                        dest.Challenges = challenges;
                    }
                });
        }
    }
}

