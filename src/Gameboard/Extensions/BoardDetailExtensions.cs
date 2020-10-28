// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Stack.Http.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gameboard
{
    /// <summary>
    /// board detail view model extension
    /// </summary>
    public static class BoardDetailExtensions
    {
        /// <summary>
        /// get challenge models by board type
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static IEnumerable<IChallengeModel> GetChallengeModels(this BoardDetail board)
        {
            if (board.BoardType == BoardType.Trivia)
            {
                return board.Categories.SelectMany(c => c.Questions);
            }

            if (board.BoardType == BoardType.Map)
            {
                return board.Maps.SelectMany(c => c.Coordinates);
            }

            throw new InvalidModelException($"Unknown board type '{board.BoardType}");
        }

        /// <summary>
        /// checks if board has started and has not ended
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsActive(this BoardDetail board)
        {
            return HasStarted(board) && HasNotEnded(board);
        }

        /// <summary>
        /// checks if board has started
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool HasStarted(this BoardDetail board)
        {
            if (board == null) return false;

            return DateTimeExtensions.HasStarted(board.StartTime);
        }

        /// <summary>
        /// checks if board has not ended
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool HasNotEnded(this BoardDetail board)
        {
            return DateTimeExtensions.HasNotEnded(board.StopTime);
        }

        /// <summary>
        /// checks if a board is active
        /// </summary>
        /// <param name="board"></param>
        /// <param name="start"></param>
        /// <param name="overrideMaxMinutes"></param>
        /// <returns></returns>
        public static bool IsActive(this BoardDetail board, DateTime? start, int? overrideMaxMinutes = null)
        {
            if (start.HasValue)
                return board.Ends(start, overrideMaxMinutes) > DateTime.UtcNow;

            return false;
        }

        /// <summary>
        /// determine end datetime for board view model
        /// </summary>
        /// <param name="board"></param>
        /// <param name="start"></param>
        /// <param name="overrideMaxMinutes"></param>
        /// <returns></returns>
        public static DateTime? Ends(this BoardDetail board, DateTime? start, int? overrideMaxMinutes = null)
        {
            if (start.HasValue)
                return start.Value.AddMinutes(overrideMaxMinutes ?? board.MaxMinutes);

            return null;
        }

        /// <summary>
        /// board extension to find board by challenge link id
        /// </summary>
        /// <param name="boards"></param>
        /// <param name="challengeLinkId"></param>
        /// <returns></returns>
        public static BoardDetail FindByChallengeLinkId(this IEnumerable<BoardDetail> boards, string challengeLinkId)
        {
            var board = boards
                .Where(b => b.BoardType == Data.BoardType.Trivia)
                .FirstOrDefault(b => b.Categories
                    .Any(ct => ct.Questions
                        .Any(c => c.ChallengeLink.Id == challengeLinkId)));

            if (board == null)
            {
                board = boards
                    .Where(b => b.BoardType == Data.BoardType.Map)
                    .FirstOrDefault(b => b.Maps
                        .Any(m => m.Coordinates
                            .Any(c => c.ChallengeLink.Id == challengeLinkId)));
            }

            return board;
        }

        /// <summary>
        /// find collection of challenges by array of challenge link ids
        /// </summary>
        /// <param name="game"></param>
        /// <param name="challengeLinkIds"></param>
        /// <returns></returns>
        public static List<ChallengeDetail> FindChallengesByChallengeLinkIds(this GameDetail game, string[] challengeLinkIds)
        {
            var challenges = new List<ChallengeDetail>();

            challenges.AddRange(game.Boards
                .Where(b => b.BoardType == Data.BoardType.Trivia)
                .SelectMany(b => b.Categories)
                .SelectMany(c => c.Questions)
                .Where(q => challengeLinkIds.Contains(q.ChallengeLink.Id))
                .Select(q => q.Challenge));

            challenges.AddRange(game.Boards
                .Where(b => b.BoardType == Data.BoardType.Map)
                .SelectMany(b => b.Maps)
                .SelectMany(m => m.Coordinates)
                .Where(c => challengeLinkIds.Contains(c.ChallengeLink.Id))
                .Select(c => c.Challenge));

            return challenges;
        }

        /// <summary>
        /// get all challenges by collection of boards
        /// </summary>
        /// <param name="boards"></param>
        /// <returns></returns>
        public static IEnumerable<ChallengeDetail> GetChallenges(this IEnumerable<BoardDetail> boards)
        {
            var challenges = new List<ChallengeDetail>();

            foreach (var board in boards)
            {
                challenges.AddRange(GetChallenges(board));
            }

            return challenges;
        }

        /// <summary>
        /// get challenges
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static IEnumerable<ChallengeDetail> GetChallenges(this BoardDetail board)
        {
            var challenges = new List<ChallengeDetail>();

            if (board.BoardType == Data.BoardType.Trivia)
            {
                challenges.AddRange(board.Categories.SelectMany(c => c.Questions).Where(q => q.Challenge != null).Select(c => c.Challenge));
            }

            if (board.BoardType == Data.BoardType.Map)
            {
                challenges.AddRange(board.Maps.SelectMany(c => c.Coordinates).Where(q => q.Challenge != null).Select(c => c.Challenge));
            }

            return challenges;
        }

        /// <summary>
        /// get challenge from board by challenge link id
        /// </summary>
        /// <param name="board"></param>
        /// <param name="challengeLinkId"></param>
        /// <returns></returns>
        public static ChallengeDetail GetChallengeById(this BoardDetail board, string challengeLinkId)
        {
            return GetChallenges(board)
                .FirstOrDefault(c => c.Id == challengeLinkId);
        }
    }
}

