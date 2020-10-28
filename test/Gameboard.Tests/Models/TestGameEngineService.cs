// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using GameEngine.Abstractions;
using GameEngine.Abstractions.Models;
using GameEngine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Problem = GameEngine.Models.Problem;

namespace Gameboard.Tests
{
    public class TestGameEngineService : IGameEngineService
    {
        public async Task<ChallengeSpec> ChallengeSpec(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<ChallengeSpec>> ChallengeSpecs()
        {
            throw new System.NotImplementedException();
        }

        public async Task ChangeVm(VmAction vmAction)
        {
            return;
        }

        public async Task Delete(string id)
        {
            return;
        }

        public async Task DeleteChallengeSpec(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SessionForecast[]> GetForecast()
        {
            return new SessionForecast[] { };
        }

        public async Task Grade(ProblemFlag flag)
        {
            return;
        }

        public async Task<Game> Load()
        {
            return new Game { };
        }

        public async Task<bool> ReserveSession(string id)
        {
            return true;
        }

        public async Task<bool> ReserveSession(SessionRequest sr)
        {
            return true;
        }

        public Task<ChallengeSpec> SaveChallengeSpec(string name, ChallengeSpec challengeSpec)
        {
            throw new System.NotImplementedException();
        }

        public async Task Spawn(Problem problem)
        {
            return;
        }

        public async Task<ConsoleSummary> Ticket(string vmId)
        {
            return new ConsoleSummary();
        }

        public async Task<bool> CancelSession(string id)
        {
            return true;
        }
    }
}
