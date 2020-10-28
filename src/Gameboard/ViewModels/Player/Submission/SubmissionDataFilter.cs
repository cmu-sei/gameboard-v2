// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class SubmissionDataFilter : IDataFilter<Submission>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Submission> FilterQuery(IQueryable<Submission> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Submission> SearchQuery(IQueryable<Submission> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query;
        }

        public IOrderedQueryable<Submission> SortQuery(IQueryable<Submission> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "timestamp";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "timestamp":
                default:
                    return desc
                        ? query.OrderByDescending(x => x.Timestamp)
                        : query.OrderBy(x => x.Timestamp);
            }
        }
    }
}

