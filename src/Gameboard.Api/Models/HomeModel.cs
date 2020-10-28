// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Models;
using System.Collections.Generic;

namespace Gameboard.Api.Models
{
    /// <summary>
    /// home model for display on api ~/
    /// </summary>
    public class HomeModel
    {
        public ApiStatus ApiStatus { get; set; }
        public List<ConfigurationItem> Configuration { get; set; } = new List<ConfigurationItem>();
        public string ApplicationName { get; set; }
    }
}

