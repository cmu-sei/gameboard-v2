// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;

namespace Gameboard.Exceptions
{
    /// <summary>
    /// exception to expose game engine connection issues
    /// </summary>
    public class GameEngineConnectionException : Exception
    {
        public string GameId { get; }

        public string GameEngineUrl { get; }

        public GameEngineConnectionException(string gameId, string gameEngineUrl)
            : base($"Failed to retrieve '{gameId}' from Game Engine at '{gameEngineUrl}'")
        {
            GameId = gameId;
            GameEngineUrl = gameEngineUrl;
        }
    }
}

