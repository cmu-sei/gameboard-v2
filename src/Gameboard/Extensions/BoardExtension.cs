// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;

namespace Gameboard
{
    /// <summary>
    /// board entity extensions
    /// </summary>
    public static class BoardExtensions
    {
        /// <summary>
        /// check if board entity has started and has not ended
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsActive(this Board board)
        {
            return HasStarted(board) && HasNotEnded(board);
        }

        /// <summary>
        /// check if board entity has started
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool HasStarted(this Board board)
        {
            if (board == null) return false;

            return DateTimeExtensions.HasStarted(board.StartTime);
        }

        /// <summary>
        /// check if board entity has not started
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool HasNotEnded(this Board board)
        {
            if (board == null) return false;

            return DateTimeExtensions.HasNotEnded(board.StopTime);
        }       
    }
}

