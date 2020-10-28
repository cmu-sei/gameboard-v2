// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class ProblemDataFilter : IDataFilter<Problem>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Problem> FilterQuery(IQueryable<Problem> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "running":
                        query = query.Where(p => p.GamespaceReady);
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Problem> SearchQuery(IQueryable<Problem> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query;
        }

        public IOrderedQueryable<Problem> SortQuery(IQueryable<Problem> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "start";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "start":
                default:
                    return desc ? query.OrderByDescending(x => x.Start) : query.OrderBy(x => x.Start);
            }
        }
    }
}

