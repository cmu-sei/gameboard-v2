// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ValidationRules;
using Stack.Validation.Attributes;

namespace Gameboard.ViewModels
{
    [Validation(
        typeof(UserIsTeamMember),
        typeof(ProblemHasVm)
    )]
    public class ProblemConsoleAction
    {
        public string Id { get; set; }
        public string VmId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}

