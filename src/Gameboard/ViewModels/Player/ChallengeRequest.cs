// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(ChallengeIsValid),
        typeof(BoardHasStarted),
        typeof(TeamIsLockedOrIsPreviewAllowed)
    )]
    public class ChallengeRequest
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
    }
}

