// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using GameEngine.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gameboard
{
    public interface IGameFactory
    {        
        GameDetail GetGame();
        IEnumerable<ChallengeSpec> ChallengeSpecs { get; }
        Task Load();
        void Clear();
        Task Refresh();
    }
}
