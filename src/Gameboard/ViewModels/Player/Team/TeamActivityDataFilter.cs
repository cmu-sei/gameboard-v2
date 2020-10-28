// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class TeamActivityDataFilter : IDataFilter<TeamActivity>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<TeamActivity> FilterQuery(IQueryable<TeamActivity> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "id":
                        query = not
                            ? query.Where(ta => ta.Id != filter.Value)
                            : query.Where(ta => ta.Id == filter.Value);
                        break;
                    default :
                        break;
                }
            }

            return query;
        }

        public IQueryable<TeamActivity> SearchQuery(IQueryable<TeamActivity> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(x => x.Name.ToLower().Contains(term));
        }

        public IOrderedQueryable<TeamActivity> SortQuery(IQueryable<TeamActivity> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "-start";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "start":
                return desc
                    ? query.OrderByDescending(c => c.Start)
                    : query.OrderBy(c => c.Start);
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);

            }
        }
    }
}

