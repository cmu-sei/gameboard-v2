// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Identity;
using Gameboard.ViewModels;

namespace Gameboard
{
    /// <summary>
    /// team extensions
    /// </summary>
    public static class TeamExtensions
    {
        /// <summary>
        /// determines whether a team name should be anonymized
        /// </summary>
        /// <param name="team"></param>
        /// <param name="options"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static bool IsAnonymizeTeam(this ITeamModel team, LeaderboardOptions options, UserIdentity identity)
        {
            if (options.Anonymize)
            {
                if (team != null && identity != null)
                {
                    return IsAnonymizeTeam(options, identity, team.Id);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// determines whether a team name should be anonymized
        /// </summary>
        /// <param name="user"></param>
        /// <param name="options"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static bool IsAnonymizeTeam(this User user, LeaderboardOptions options, UserIdentity identity)
        {
            if (options.Anonymize)
            {
                if (user != null && identity != null)
                {
                    return IsAnonymizeTeam(options, identity, user.TeamId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// determines whether a team name should be anonymized
        /// </summary>
        /// <param name="options"></param>
        /// <param name="identity"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        static bool IsAnonymizeTeam(LeaderboardOptions options, UserIdentity identity, string teamId)
        {
            var identityUser = identity.User;
            var anonymize = options?.Anonymize ?? true;

            if (!anonymize) return false;

            if (identityUser == null) return true;

            return !identityUser.IsModerator && !identityUser.IsObserver && identityUser.TeamId != teamId;
        }

        /// <summary>
        /// converts a team name to the anonymized team name using the pattern:
        /// <see cref="Team.OrganizationName"/>-<see cref="GameDetail.AnonymizeTag"/>-<see cref="Team.Number"/>
        /// </summary>
        /// <param name="team"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static string AnonymizeTeamName(this ITeamModel team, GameDetail game)
        {
            return AnonymizeTeamName(team?.OrganizationName, game.AnonymizeTag, team?.Number ?? 0);
        }
        
        /// <summary>
        /// converts team parameters to pattern
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="tag"></param>
        /// <param name="teamNumber"></param>
        /// <returns></returns>
        public static string AnonymizeTeamName(string organizationName, string tag, int teamNumber)
        {
            if (string.IsNullOrWhiteSpace(organizationName))
                return "N/A";

            return $"{organizationName}-{tag}-{teamNumber}";
        }
    }
}

