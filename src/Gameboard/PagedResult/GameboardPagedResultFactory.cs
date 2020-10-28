// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard
{
    /// <summary>
    /// custom paged result factory with async and non-async Executions support
    /// </summary>
    public class GameboardPagedResultFactory
    {
        public IMapper Mapper { get; }
        public GameboardPagedResultFactory(IMapper mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// execute query async to created a paged result
        /// </summary>
        /// <typeparam name="TEntityModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="dataFilter"></param>
        /// <param name="identity"></param>
        /// <param name="mappingOperationOptions">objects to inject into the auto mapper configuration for use during mapping</param>
        /// <returns></returns>
        public async Task<PagedResult<TEntityModel, TViewModel>> ExecuteAsync<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter,
            IStackIdentity identity,
            Action<IMappingOperationOptions> mappingOperationOptions = null)
            where TEntityModel : class
            where TViewModel : class
        {
            var result = ProcessDataFilter<TEntityModel, TViewModel>(ref query, dataFilter, identity);

            var entities = await query.ToListAsync();

            result.Results = mappingOperationOptions == null
                    ? entities.Select(Mapper.Map<TEntityModel, TViewModel>).ToArray()
                    : entities.Select(e => Mapper.Map<TEntityModel, TViewModel>(e, mappingOperationOptions)).ToArray();

            return result;
        }

        /// <summary>
        /// execute query to created a paged result
        /// </summary>
        /// <typeparam name="TEntityModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="dataFilter"></param>
        /// <param name="identity"></param>
        /// <param name="mappingOperationOptions"></param>
        /// <returns></returns>
        public PagedResult<TEntityModel, TViewModel> Execute<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter,
            IStackIdentity identity,
            Action<IMappingOperationOptions> mappingOperationOptions = null)
            where TEntityModel : class
            where TViewModel : class
        {
            var result = ProcessDataFilter<TEntityModel, TViewModel>(ref query, dataFilter, identity);

            var entities = query.ToList();

            result.Results = mappingOperationOptions == null
                    ? entities.Select(Mapper.Map<TEntityModel, TViewModel>).ToArray()
                    : entities.Select(e => Mapper.Map<TEntityModel, TViewModel>(e, mappingOperationOptions)).ToArray();

            return result;
        }

        static PagedResult<TEntityModel, TViewModel> ProcessDataFilter<TEntityModel, TViewModel>(ref IQueryable<TEntityModel> query, IDataFilter<TEntityModel> dataFilter, IStackIdentity identity)
            where TEntityModel : class
            where TViewModel : class
        {
            var result = new PagedResult<TEntityModel, TViewModel>
            {
                DataFilter = dataFilter
            };

            query = query.AsNoTracking();

            if (dataFilter != null)
            {
                query = dataFilter.SearchQuery(query);
                query = dataFilter.FilterQuery(query, identity);
            }

            result.Total = query.Count();

            if (dataFilter != null)
            {
                query = dataFilter.SortQuery(query);

                if (dataFilter.Skip > 0) query = query.Skip(dataFilter.Skip);
                if (dataFilter.Take > 0) query = query.Take(dataFilter.Take);
            }

            return result;
        }
    }
}

