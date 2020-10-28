// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using System;
using System.Collections.Generic;

namespace Gameboard.ViewModels
{

    public class CategoryEdit
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }

        public List<QuestionEdit> Questions { get; set; } = new List<QuestionEdit>();
    }
}

