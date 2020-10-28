// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gameboard.Data
{
    public class Team
    {
        [Key]
        [MaxLength(40)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string InviteCode { get; set; }

        public ICollection<User> Users { get; set; } = new HashSet<User>();

        public bool IsLocked { get; set; }

        public bool IsDisabled { get; set; }

        public string Badges { get; set; }

        [MaxLength(40)]
        [Required(AllowEmptyStrings = false)]
        public string OwnerUserId { get; set; }

        public string OrganizationLogoUrl { get; set; }

        public string OrganizationalUnitLogoUrl { get; set; }

        public string OrganizationName { get; set; }

        public int Number { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        public ICollection<TeamBoard> TeamBoards { get; set; } = new HashSet<TeamBoard>();
    }
}

