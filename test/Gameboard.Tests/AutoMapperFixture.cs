// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using AutoMapper;
using Stack.Http;
using Xunit;

namespace Gameboard.Tests
{
    /// <summary>
    /// Important note: Fixtures can be shared across assemblies, but collection definitions must be in the same assembly as the test that uses them.
    /// </summary>
    [CollectionDefinition("AutoMapper")]
    public class AutoMapperCollection : ICollectionFixture<AutoMapperFixture>
    {
    }


    public class AutoMapperFixture : IDisposable
    {
        public AutoMapperFixture()
        {
            var type = typeof(Profile);

            var config = new MapperConfiguration(cfg => {
                type.ProcessTypeOf("Gameboard", (profile) => {
                    if (Activator.CreateInstance(profile) is Profile instance)
                    {
                        cfg.AddProfile(instance);
                    }
                });
            });
        }

        public void Dispose() { }
    }
}

