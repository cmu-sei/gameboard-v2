// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard.Options
{
    /// <summary>
    /// security header options with default configuration
    /// </summary>
    public class SecurityHeaderOptions
    {
        public string ContentSecurity { get; set; } = "default-src 'self' 'unsafe-inline'";
        public string XContentType { get; set; } = "nosniff";
        public string XFrame { get; set; } = "SAMEORIGIN";
        public CorsPolicyOptions Cors { get; set; }
    }
}

