// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.ViewModels
{    public class ParticipationReport
    {
        public int TeamRegistrantCount { get; set; }

        public int IndividualRegistrantCount { get; set; }

        public int TeamCount { get; set; }

        public List<ParticipationReportOrganization> Organizations { get; set; } = new List<ParticipationReportOrganization>();
        public string Text { get; set; } = "Teams";
    }

    public class ParticipationReportOrganization
    {
        public string Name { get; set; }
        public int TeamCount { get; set; }
    }
}

