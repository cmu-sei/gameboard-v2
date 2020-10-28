// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Newtonsoft.Json;

namespace Gameboard
{
    /// <summary>
    /// extensions for objects
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// serialize and deserialize object to get a copy
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static TObject Clone<TObject>(this TObject o)
            where TObject : class
        {
            return JsonConvert.DeserializeObject<TObject>(JsonConvert.SerializeObject(o,
                Formatting.Indented,  new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}

