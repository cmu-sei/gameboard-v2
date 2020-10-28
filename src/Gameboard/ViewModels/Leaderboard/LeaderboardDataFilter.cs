// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard
{
    public class LeaderboardDataFilter : IDataFilter<LeaderboardScore>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<LeaderboardScore> FilterQuery(IQueryable<LeaderboardScore> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                var value = (filter.Value ?? "").ToLower().Replace(" ", "");

                switch (key)
                {
                    case "organization":
                        query = not
                            ? query.Where(x => !x.OrganizationName.ToLower().Replace(" ", "").Contains(value))
                            : query.Where(x => x.OrganizationName.ToLower().Replace(" ", "").Contains(value));
                        break;
                    case "team":
                        query = not
                            ? query.Where(x => !x.Name.ToLower().Replace(" ", "").Contains(value))
                            : query.Where(x => x.Name.ToLower().Replace(" ", "").Contains(value));
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<LeaderboardScore> SearchQuery(IQueryable<LeaderboardScore> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = (Term ?? "").ToLower().Trim().Replace(" ", "");

            return query.Where(x => x.Name.ToLower().Replace(" ", "").Contains(term));
        }

        public IOrderedQueryable<LeaderboardScore> SortQuery(IQueryable<LeaderboardScore> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "rank";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "duration":
                    return desc
                        ? query.OrderByDescending(c => c.Duration)
                        : query.OrderBy(c => c.Duration);
                case "organization":
                    return desc
                        ? query.OrderByDescending(c => c.OrganizationName)
                        : query.OrderBy(c => c.OrganizationName);
                case "team":
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                case "rank":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Rank)
                        : query.OrderBy(c => c.Rank);
            }
        }
    }
}

