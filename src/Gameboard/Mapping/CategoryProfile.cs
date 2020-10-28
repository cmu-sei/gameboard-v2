// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using System.Linq;

namespace Gameboard.Mapping
{
    public class CategoryProfile : AutoMapper.Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDetail>()
                .AfterMap((s, d) =>
                {
                    d.Questions = d.Questions.OrderBy(c => c.Order).ToList();
                });

            CreateMap<CategoryEdit, Category>();
        }
    }
}

