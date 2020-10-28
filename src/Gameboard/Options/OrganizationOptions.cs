// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.ViewModels;
using System.Collections.Generic;

namespace Gameboard
{
    /// <summary>
    /// organization options including organization collections for enrollment
    /// </summary>
    public class OrganizationOptions
    {
        public bool IsEnabled { get; set; }
        public string ClaimKey { get; set; }
        public List<OrganizationDetail> Items { get; set; } = new List<OrganizationDetail>();
    }
}

