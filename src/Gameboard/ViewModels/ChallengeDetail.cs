// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using GameEngine.Abstractions.Models;
using GameEngine.Models;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class ChallengeDetail
    {
        /// <summary>
        /// set by <see cref="IGameFactory.Load()"/>
        /// </summary>
        public string Id { get; set; }        
        /// <summary>
        /// set by <see cref="IGameFactory.Load()"/>
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public string Slug { get; set; }
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public FlagStyle FlagStyle { get; set; }
        /// <summary>
        /// mapped from <see cref="ChallengeSpec"/>
        /// </summary>
        public bool IsMultiStage { get; set; }

        public bool IsMultiPart 
        { 
            get { return TokenCount > 1 && !IsMultiStage; } 
        }

        /// <summary>
        /// set by <see cref="Mapping.ChallengeSpecProfile"/>
        /// </summary>
        public int TokenCount { get; set; }
        /// <summary>
        /// mapped from <see cref="Mapping.ChallengeSpecProfile"/>
        /// </summary>
        public int FlagCount { get; set; }

        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public string ProblemId { get; set; }

        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public string ProblemStatus { get; set; }

        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public double ProblemScore { get; set; }

        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public double TotalMinutes { get; set; }
        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public bool GamespaceReady { get; set; }
        /// <summary>
        /// set by <see cref="Services.BoardService.Get(BoardRequest)"/>
        /// </summary>
        public bool HasGamespace { get; set; }        
    }
}

