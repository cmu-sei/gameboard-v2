// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;

namespace Gameboard.ViewModels
{
    public class ChallengeProblem
    {
        public ChallengeDetail Challenge { get; set; }
        public ProblemDetail Problem { get; set; }
        public BoardDetail Board { get; set; }
    }

}

