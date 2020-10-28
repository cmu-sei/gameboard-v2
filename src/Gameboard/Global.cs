// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard
{
    public static class Permission
    {
        public const string Moderator = "moderator";
        public const string Observer = "observer";
        public const string ChallengeDeveloper = "challengedeveloper";
        public const string GameDesigner = "gamedesigner";
    }

    public static class CacheType
    {
        public const string Default = "Default";
        public const string Redis = "Redis";
    }

    public static class MappingKeys
    {
        public const string Identity = "Identity";
        public const string GameFactory = "GameFactory";
        public const string GameEngineService = "GameEngineService";
        public const string LeaderboardOptions = "LeaderboardOptions";
    }

    public static class CacheKeys
    {
        public const string Game = "Game";
        public const string ChallengeSpecs = "ChallengeSpecs";
    }

    public static class EnvironmentMode
    {
        public const string Default = "Default";
        public const string Test = "Test";
    }
}

