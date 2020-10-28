// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Gameboard.ViewModels
{
    public class TeamDataFilter : IDataFilter<Team>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Team> FilterQuery(IQueryable<Team> query, IStackIdentity identity)
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
                    case "organization":
                        query = not
                            ? query.Where(x => !x.OrganizationName.ToLower().Replace(" ", "").Contains(value))
                            : query.Where(x => x.OrganizationName.ToLower().Replace(" ", "").Contains(value));
                        break;
                    case "board":
                        query = not
                            ? query.Where(x => !x.TeamBoards.Any(tb => values.Contains(tb.BoardId.ToLower())))
                            : query.Where(x => x.TeamBoards.Any(tb => values.Contains(tb.BoardId.ToLower())));
                        break;
                    case "badge":

                        foreach (var v in values)
                        {
                            query = query.Where(t => t.Badges.ToLower().Contains(v.ToLower().Trim()));
                        }
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Team> SearchQuery(IQueryable<Team> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim().Replace(" ", "");

            return query.Where(x =>
                x.Name.ToLower().Replace(" ", "").Contains(term) ||
                x.OrganizationName.ToLower().Replace(" ", "").Contains(term) ||
                x.Users.Any(u => u.Name.ToLower().Replace(" ", "").Contains(term)) ||
                x.Number.ToString() == term
            );
        }

        public IOrderedQueryable<Team> SortQuery(IQueryable<Team> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var boardId = string.Empty;

            if (sort.StartsWith("board") && Sort.Contains("="))
            {
                boardId = Sort.Split('=')[1];
                sort = "board";
            }

            switch (sort)
            {
                case "organization":
                    return desc
                        ? query.OrderByDescending(c => c.OrganizationName)
                        : query.OrderBy(c => c.OrganizationName);
                case "created":
                    return desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                case "start":
                    return desc
                        ? query.OrderByDescending(c => c.TeamBoards.Select(tb => tb.Start).FirstOrDefault())
                        : query.OrderBy(c => c.TeamBoards.Select(tb => tb.Start).FirstOrDefault());
                case "board":                    
                    return desc
                        ? query.OrderByDescending(team =>
                            team.TeamBoards.Any(tb => tb.BoardId == boardId)
                                ? team.TeamBoards.Where(tb => tb.BoardId == boardId).Select(tb => tb.Score).FirstOrDefault()
                                : -1.0)
                        : query.OrderBy(team =>
                            team.TeamBoards.Any(tb => tb.BoardId == boardId)
                                ? team.TeamBoards.Where(tb => tb.BoardId == boardId).Select(tb => tb.Score).FirstOrDefault()
                                : -1.0);                    
                case "locked":
                    return desc
                        ? query.OrderByDescending(c => c.IsLocked)
                        : query.OrderBy(c => c.IsLocked);
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}

