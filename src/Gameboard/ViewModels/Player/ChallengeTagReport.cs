// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class ChallengeTagReport
    {
        public string Name { get; set; }

        public PagedResult<ChallengeDetail, ChallengeDetail> Result { get; set; } = new PagedResult<ChallengeDetail, ChallengeDetail>();
    }

    public class ChallengeTagReportDataFilter : IDataFilter<ChallengeDetail>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<ChallengeDetail> FilterQuery(IQueryable<ChallengeDetail> query, IStackIdentity identity)
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

        public IQueryable<ChallengeDetail> SearchQuery(IQueryable<ChallengeDetail> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim().Replace(" ", "");

            return query.Where(x =>
                x.Title.ToLower().Replace(" ", "").Contains(term)
            );
        }

        public IOrderedQueryable<ChallengeDetail> SortQuery(IQueryable<ChallengeDetail> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "title";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var boardId = string.Empty;

            switch (sort)
            {                
                case "title":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title);
            }
        }
    }
}

