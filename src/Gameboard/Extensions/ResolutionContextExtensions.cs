// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Stack.Http.Identity;

namespace Gameboard
{
    /// <summary>
    /// automapper mapping resolution context extensions
    /// </summary>
    public static class ResolutionContextExtensions
    {
        /// <summary>
        /// get identity from resolution context by <see cref="MappingKeys.Identity"/>
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static IStackIdentity GetIdentity(this ResolutionContext res)
        {
            return GetType<IStackIdentity>(res, MappingKeys.Identity);
        }

        /// <summary>
        /// get game factory from resolution context by <see cref="MappingKeys.GameFactory"/>
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static IGameFactory GetGameFactory(this ResolutionContext res)
        {
            return GetType<IGameFactory>(res, MappingKeys.GameFactory);
        }        

        /// <summary>
        /// get object from resolution context by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="res"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetType<T>(this ResolutionContext res, string key)
            where T : class
        {
            if (res.Items.ContainsKey(key))
            {
                return res.Items[key] as T;
            }

            return null;
        }
    }
}

