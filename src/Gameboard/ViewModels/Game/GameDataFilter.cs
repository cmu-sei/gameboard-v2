// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class GameDataFilter : IDataFilter<Game>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Game> FilterQuery(IQueryable<Game> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");

                switch (key)
                {                    
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Game> SearchQuery(IQueryable<Game> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim().Replace(" ", "");

            return query.Where(x =>
                (x.Name != null && x.Name.ToLower().Replace(" ", "").Contains(term))
            );
        }

        public IOrderedQueryable<Game> SortQuery(IQueryable<Game> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}

