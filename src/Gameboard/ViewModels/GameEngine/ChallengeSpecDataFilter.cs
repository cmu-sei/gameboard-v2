// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using GameEngine.Abstractions.Models;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class ChallengeSpecDataFilter : IDataFilter<ChallengeSpec>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<ChallengeSpec> FilterQuery(IQueryable<ChallengeSpec> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");
                var values = filter.StringValues;
                var value = (filter.Value ?? "").ToLower().Replace(" ", "");

                switch (key)
                {
                    case "multipart":
                        query = not
                            ? query.Where(x => !x.Flags.Any(f => f.Tokens.Count() > 1))
                            : query.Where(x => x.Flags.Any(f => f.Tokens.Count() > 1));
                        break;
                    case "multistage":
                        query = not
                            ? query.Where(x => !x.IsMultiStage)
                            : query.Where(x => x.IsMultiStage);
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<ChallengeSpec> SearchQuery(IQueryable<ChallengeSpec> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim().Replace(" ", "");

            return query.Where(x =>
                (x.Slug != null && x.Slug.ToLower().Replace(" ", "").Contains(term)) ||
                (x.Title != null && x.Title.ToLower().Replace(" ", "").Contains(term)) ||
                (x.Tags != null && x.Tags.ToLower().Replace(" ", "").Replace("-", "").Replace("_", "").Contains(term)) ||
                (x.Description != null && x.Description.ToLower().Replace(" ", "").Contains(term)) ||
                (x.Text != null && x.Text.ToLower().Replace(" ", "").Contains(term))
            );
        }

        public IOrderedQueryable<ChallengeSpec> SortQuery(IQueryable<ChallengeSpec> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "title":
                    return desc
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title);
                case "slug":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Slug)
                        : query.OrderBy(c => c.Slug);
            }
        }
    }
}

