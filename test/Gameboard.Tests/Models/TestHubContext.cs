// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Gameboard.Tests
{
    public class TestHubContext<THub, T> : IHubContext<THub, T>
        where THub : Hub<T>
        where T : class
    {
        public IHubClients<T> Clients
        {
            get
            {
                var mock = new Mock<IHubClients<T>>();
                return mock.Object;
            }
        }

        public IGroupManager Groups
        {
            get
            {
                var mock = new Mock<IGroupManager>();
                return mock.Object;
            }
        }
    }
}

