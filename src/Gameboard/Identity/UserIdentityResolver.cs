// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Cache;
using Gameboard.Data;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stack.Http.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Gameboard.Identity
{
    /// <summary>
    /// user identity resolver
    /// </summary>
    public class UserIdentityResolver : HttpIdentityResolver
    {
        GameboardDbContext DbContext { get; }
        ILogger<UserIdentityResolver> Logger { get; }
        IGameboardCache<User> UserCache { get; }

        /// <summary>
        /// creates an instance of UserIdentityResolver
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="db"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public UserIdentityResolver(IHttpContextAccessor httpContextAccessor, GameboardDbContext db, IGameboardCache<User> cache, ILogger<UserIdentityResolver> logger)
            : base(httpContextAccessor)
        {
            DbContext = db;
            UserCache = cache;
            Logger = logger;
        }

        /// <summary>
        /// get identity
        /// </summary>
        /// <returns></returns>
        public override async Task<IStackIdentity> GetIdentityAsync()
        {
            var httpContext = HttpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            var claimsPrincipal = httpContext.User;

            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            if (string.IsNullOrWhiteSpace(clientId))
                return null;

            return Get(claimsPrincipal) ?? Add(claimsPrincipal) ?? Update(claimsPrincipal);
        }

        /// <summary>
        /// add new user and store in user cache
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        IStackIdentity Add(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            var user = new User
            {
                Id = subject.ToLower(),
                Name = name ?? "Anonymous"
            };

            DbContext.Users.Add(user);
            DbContext.SaveChanges();

            UserCache.Set(subject, user);

            return ConvertToIdentity(user, claimsPrincipal);
        }

        /// <summary>
        /// get user from cache or database and cache
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        IStackIdentity Get(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            var user = UserCache.Get(subject);

            if (user == null)
            {
                user = DbContext.Users.SingleOrDefault(p => p.Id.ToLower() == subject);

                if (user == null)
                    return null;

                UserCache.Set(subject, user);
            }

            return ConvertToIdentity(user, claimsPrincipal);
        }

        /// <summary>
        /// update user data and remove from cache
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        IStackIdentity Update(ClaimsPrincipal claimsPrincipal)
        {
            string subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            var user = DbContext.Users.SingleOrDefault(p => p.Id.ToLower() == subject);

            if (user.Name != name)
            {
                user.Name = name;
                DbContext.SaveChanges();
                UserCache.Remove(user.Id);
            }

            return ConvertToIdentity(user, claimsPrincipal);
        }

        /// <summary>
        /// convert user to IStackIdentity
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        IStackIdentity ConvertToIdentity(User user, ClaimsPrincipal claimsPrincipal)
        {
            var data = new List<object>() { user, claimsPrincipal };

            var permissions = new List<string>();

            if (user.IsModerator) permissions.Add(Permission.Moderator);
            if (user.IsObserver) permissions.Add(Permission.Observer);
            if (user.IsChallengeDeveloper) permissions.Add(Permission.ChallengeDeveloper);
            if (user.IsGameDesigner) permissions.Add(Permission.GameDesigner);

            return new UserIdentity 
            { 
                Id = user.Id, 
                User = user, 
                ClaimsPrincipal = claimsPrincipal, 
                Data = data, 
                Permissions = permissions.ToArray() 
            };
        }
    }
}

