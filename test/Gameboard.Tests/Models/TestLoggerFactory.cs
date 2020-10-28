// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.Extensions.Logging;
using System;

namespace Gameboard.Tests
{

    public class TestLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(string context)
        {
            return new TestLogger();
        }

        public ILogger CreateLogger(Type context)
        {
            return new TestLogger(context.Name);
        }

        public void AddProvider(ILoggerProvider provider)
        {

        }

        public void Dispose()
        {

        }
    }
}

