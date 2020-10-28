// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

namespace Gameboard
{
    /// <summary>
    /// options for mail
    /// </summary>
    public class MailOptions
    {
        public string Authority { get; set; }

        public string AuthorizationScope { get; set; }

        public string Endpoint { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }
    }
}

