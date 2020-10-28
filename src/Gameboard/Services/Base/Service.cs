// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Identity;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// base service class with support for:
    /// <see cref="IStackIdentityResolver"/>, <see cref="ValidationHandler"/>, <see cref="GameboardPagedResultFactory"/>, and <see cref="IGameFactory"/>
    /// </summary>
    public abstract class Service
    {

        protected readonly IStackIdentityResolver _identityResolver;
        protected IMapper Mapper { get; }

        protected IValidationHandler ValidationHandler { get; }

        protected GameboardPagedResultFactory PagedResultFactory { get; }

        protected LeaderboardOptions LeaderboardOptions { get; }

        public IGameFactory GameFactory { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="gameFactory"></param>
        /// <param name="leaderboardOptions"></param>
        protected Service(
            IStackIdentityResolver identityResolver, 
            IMapper mapper, 
            IValidationHandler validationHandler, 
            IGameFactory gameFactory, 
            LeaderboardOptions leaderboardOptions)
        {
            _identityResolver = identityResolver;
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            ValidationHandler = validationHandler ?? throw new ArgumentNullException(nameof(validationHandler));
            PagedResultFactory = new GameboardPagedResultFactory(mapper);
            GameFactory = gameFactory;
            LeaderboardOptions = leaderboardOptions;
        }

        UserIdentity _identity;
        public UserIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    try { _identity = _identityResolver.GetIdentityAsync().Result as UserIdentity; }
                    catch { }
                }

                return _identity;
            }
        }

        public bool IsModerator => Identity?.User?.IsModerator ?? false;

        public bool IsObserver => Identity?.User?.IsObserver ?? false;

        /// <summary>
        /// default mapping operations with Identity
        /// </summary>
        /// <returns></returns>
        public virtual Action<IMappingOperationOptions> GetMappingOperationOptions()
        {
            return ToMappingOperations(new Dictionary<string, object>()
            {
                { MappingKeys.Identity, Identity },
                { MappingKeys.GameFactory, GameFactory },
                { MappingKeys.LeaderboardOptions, LeaderboardOptions }
            });
        }

        /// <summary>
        /// convert dictionary to mapping operation options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Action<IMappingOperationOptions> ToMappingOperations(IDictionary<string, object> options)
        {
            return opts =>
            {
                if (options != null)
                {
                    foreach (var o in options)
                    {
                        opts.Items[o.Key] = o.Value;
                    }
                }
            };
        }

        /// <summary>
        /// map with default mapping operations
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TType Map<TType>(object source)
        {
            return Map<TType>(source, GetMappingOperationOptions());
        }

        /// <summary>
        /// map object to type with operation options
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public TType Map<TType>(object source, Action<IMappingOperationOptions> opts)
        {
            try
            {
                if (opts == null)
                    return Mapper.Map<TType>(source);

                return Mapper.Map<TType>(source, opts);
            }
            catch (AutoMapperMappingException ex)
            {
                string message = string.Format("Could not map '{0}' to '{1}'",
                    source.GetType().Name, typeof(TType).Name);

                throw new AutoMapperMappingException(message, ex);
            }
        }

        /// <summary>
        /// get a paged result by generic type and always add Identity for automapper options
        /// </summary>
        /// <typeparam name="TEntityModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="dataFilter"></param>
        /// <param name="mappingOperationOptions"></param>
        /// <returns></returns>
        public async Task<PagedResult<TEntityModel, TViewModel>> PagedResult<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter,
            Action<IMappingOperationOptions> mappingOperationOptions = null)
            where TEntityModel : class
            where TViewModel : class
        {
            return await PagedResultFactory.ExecuteAsync<TEntityModel, TViewModel>(query, dataFilter, Identity,
                mappingOperationOptions ?? GetMappingOperationOptions());
        }

        public byte[] ConvertToBytes<T>(IEnumerable<T> collection)
        {
            var value = ServiceStack.StringExtensions.ToCsv(collection);

            return Encoding.UTF8.GetBytes(value.ToString());
        }
    }
}

