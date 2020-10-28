// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.


using System.Collections.Generic;

namespace Gameboard.ViewModels
{
    public class CategoryDetail
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }

        public List<QuestionDetail> Questions { get; set; } = new List<QuestionDetail>();
    }
}

