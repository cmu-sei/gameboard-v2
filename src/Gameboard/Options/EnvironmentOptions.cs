// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard
{
    /// <summary>
    /// environment options to determine "Test" mode
    /// </summary>
    /// <remarks>
    /// This allows a flag index to be set from client processing models
    /// <see cref="ViewModels.ChallengeStart"/>
    /// <see cref="ViewModels.ChallengeReset"/>
    /// <see cref="ViewModels.UserReset"></see>
    /// <see cref="ViewModels.TeamReset"></see>
    /// </remarks>
    public class EnvironmentOptions
    {
        public string Mode { get; set; } = "Default";
        public double ResetMinutes { get; set; } = 2.0;
    }
}

