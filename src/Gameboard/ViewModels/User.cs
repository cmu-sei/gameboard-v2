// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ValidationRules;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Stack.Validation.Attributes;
using System.Linq;

namespace Gameboard.ViewModels
{
    public class UserDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string AnonymizedTeamName { get; set; }
        public bool IsModerator { get; set; }
        public bool IsObserver { get; set; }
        public bool IsChallengeDeveloper { get; set; }
        public bool IsGameDesigner { get; set; }
    }

    [Validation(
        typeof(UserIsActingUser),
        typeof(TeamOrganizationNameValid),
        typeof(EnrollmentPeriodHasNotEnded)
    )]
    public class UserEdit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
    }

    [Validation(
        typeof(IsTestMode),
        typeof(TeamHasNoTeamBoards),
        typeof(UserIsNotActingUser)
    )]
    public class UserReset
    {
        public string Id { get; set; }
    }

    public class UserDataFilter : IDataFilter<User>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<User> FilterQuery(IQueryable<User> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                var value = filter.Value.ToLower();

                switch (key)
                {
                    case "organization":
                        query = not
                            ? query.Where(u => !u.Organization.ToLower().Contains(value))
                            : query.Where(u => u.Organization.ToLower().Contains(value));
                        break;
                    case "moderator":
                        query = not
                            ? query.Where(u => !u.IsModerator)
                            : query.Where(u => u.IsModerator);
                        break;
                    case "observer":
                        query = not
                            ? query.Where(u => !u.IsObserver)
                            : query.Where(u => u.IsObserver);
                        break;
                    case "challengedeveloper":
                        query = not
                            ? query.Where(u => !u.IsChallengeDeveloper)
                            : query.Where(u => u.IsChallengeDeveloper);
                        break;
                    case "gamedesigner":
                        query = not
                            ? query.Where(u => !u.IsGameDesigner)
                            : query.Where(u => u.IsGameDesigner);
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<User> SearchQuery(IQueryable<User> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(x => x.Name.ToLower().Contains(term));
        }

        public IOrderedQueryable<User> SortQuery(IQueryable<User> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "organization":
                    return desc
                        ? query.OrderByDescending(c => c.Organization)
                        : query.OrderBy(c => c.Organization);
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}

