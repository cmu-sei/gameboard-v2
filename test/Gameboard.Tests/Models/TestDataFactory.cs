// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;

namespace Gameboard.Tests
{
    public class TestDataFactory
    {
        TestContext TestContext { get; set; }
        GameboardDbContext DbContext { get; set; }

        public TestDataFactory(TestContext context)
        {
            TestContext = context;
            DbContext = context.DbContext;
        }
    }
}
