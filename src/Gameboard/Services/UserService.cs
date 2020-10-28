// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Cache;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.ViewModels;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// User service
    /// </summary>
    public class UserService : Service<IUserRepository, User>
    {
        IGameboardCache<User> UserCache { get; }
        MailOptions MailOptions { get; }

        /// <summary>
        /// create an instance of user service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="userCache"></param>
        /// <param name="mailOptions"></param>
        /// <param name="leaderboardOptions"></param>
        public UserService(
            IStackIdentityResolver identityResolver,
            IUserRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,
            IGameboardCache<User> userCache,
            MailOptions mailOptions,
            LeaderboardOptions leaderboardOptions,
            IGameFactory gameFactory)
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            UserCache = userCache;
            MailOptions = mailOptions;
        }

        /// <summary>
        /// get all Users
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<User, UserDetail>> GetAll(UserDataFilter dataFilter = null)
        {
            if (!(Identity.User.IsModerator || Identity.User.IsObserver))
                throw new UnauthorizedAccessException("Action requires elevated permissions.");

            return await PagedResult<User, UserDetail>(Repository.GetAll(), dataFilter, GetMappingOperationOptions());
        }

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserDetail> GetById(string id)
        {
            if (!Identity.User.IsModerator && Identity.Id != id)
                throw new UnauthorizedAccessException("Action requires elevated permissions.");

            var user = UserCache.Get(id)
                ?? await Repository.GetById(id);

            if (user == null)
                throw new EntityNotFoundException($"User '{id}' was not found.");

            UserCache.Set(user.Id, user);
            return Map<UserDetail>(user);
        }

        /// <summary>
        /// toggle user permission
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<UserDetail> TogglePermission(string id, string permission)
        {
            if (!Identity.User.IsModerator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var entity = await Repository.GetById(id);

            switch (permission)
            {
                case Permission.ChallengeDeveloper:
                    entity.IsChallengeDeveloper = !entity.IsChallengeDeveloper;
                    break;
                case Permission.Moderator:
                    if (Identity.User.Id == id)
                        throw new EntityPermissionException("You cannot remove your own Moderator permissions.");
                    entity.IsModerator = !entity.IsModerator;
                    break;
                case Permission.Observer:
                    entity.IsObserver = !entity.IsObserver;
                    break;
                case Permission.GameDesigner:
                    entity.IsGameDesigner = !entity.IsGameDesigner;
                    break;
            }

            var saved = await Repository.Update(entity);
            UserCache.Set(saved.Id, saved);
            return Map<UserDetail>(saved);
        }


        /// <summary>
        /// update user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserDetail> Update(UserEdit model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var entity = await Repository.GetById(model.Id);
            entity.Name = model.Name;
            entity.Organization = model.Organization;
            var saved = await Repository.Update(entity);

            UserCache.Set(saved.Id, saved);

            return Map<UserDetail>(saved);
        }

        /// <summary>
        /// reset user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserDetail> Reset(UserReset model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var user = await Repository.GetById(model.Id);

            if (user.Team != null)
            {
                Repository.DbContext.Teams.Remove(user.Team);
            }

            user.Organization = string.Empty;
            user.TeamId = null;
            user.IsChallengeDeveloper = false;
            user.IsGameDesigner = false;
            user.IsModerator = false;
            user.IsObserver = false;
            user.Survey = string.Empty;

            UserCache.Remove(user.Id);
            await Repository.Update(user);

            return await GetById(user.Id);
        }

        async Task<TokenResponse> GetTokenAsync()
        {
            var discoveryClient = await DiscoveryClient.GetAsync(MailOptions.Authority);

            if (discoveryClient.IsError)
                throw new SecurityTokenException(discoveryClient.Error);

            var client = new TokenClient(discoveryClient.TokenEndpoint, MailOptions.ClientId, MailOptions.ClientSecret);
            var response = await client.RequestClientCredentialsAsync(MailOptions.AuthorizationScope);

            if (response.IsError)
                throw new SecurityTokenException(response.Error);

            return response;
        }

        public async Task<bool> SendMessage(MessageCreate message)
        {
            if (!Identity.User.IsModerator)
                throw new UnauthorizedAccessException("Action requires elevated permissions.");

            if (message == null)
                throw new InvalidModelException("Message is null.");

            if (message.To == null || message.To.Count == 0)
                throw new InvalidModelException("Message has no recipients.");

            if (string.IsNullOrWhiteSpace(message.Subject))
                throw new InvalidModelException("Message has no subject.");

            if (string.IsNullOrWhiteSpace(message.Body))
                throw new InvalidModelException("Message has no body.");

            var mediaType = "application/json";

            using (var client = new HttpClient { BaseAddress = new Uri(MailOptions.Authority) })
            {
                client.SetBearerToken((await GetTokenAsync()).AccessToken);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                var response = await client.PostAsync(MailOptions.Endpoint, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, mediaType));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}

