// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gameboard.Api
{
    /// <summary>
    /// seed data factory attempts to map json files to their generic types 
    /// </summary>
    public class SeedDataFactory
    {
        IHostingEnvironment HostingEnvironment { get; }
        GameboardDbContext DbContext { get; }
        SeedOptions SeedOptions { get; }

        public SeedDataFactory(IHostingEnvironment hostingEnvironment, GameboardDbContext dbContext, SeedOptions seedOptions)
        {
            HostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            SeedOptions = seedOptions ?? throw new ArgumentNullException(nameof(seedOptions));
        }

        /// <summary>
        /// deletes files after seeding succeeds
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool DeleteFile(string fileName)
        {
            var path = GetFilePath(fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        string GetFilePath(string fileName)
        {
            return Path.Combine(HostingEnvironment.ContentRootPath, SeedOptions.Path, fileName);
        }

        T ConvertFileDataTo<T>(string fileName)
            where T : class, new()
        {
            var path = GetFilePath(fileName);

            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }

            return new T();
        }

        /// <summary>
        /// seeds the database with a collection of the specified type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lookup"></param>
        /// <param name="remove"></param>
        /// <returns></returns>
        public SeedDataResult<TEntity> Seed<TEntity>(string fileName, Func<TEntity, TEntity> lookup, Action<TEntity> remove)
            where TEntity : class, new()
        {
            var result = new SeedDataResult<TEntity>();

            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentNullException(nameof(fileName));

                if (lookup == null)
                    throw new ArgumentNullException(nameof(lookup));

                result.Entities = ConvertFileDataTo<List<TEntity>>(fileName);

                if (result.Entities.Any())
                {
                    foreach (var entity in result.Entities)
                    {
                        var item = new SeedDataResultItem<TEntity>
                        {
                            Entity = lookup(entity)
                        };

                        if (item.Entity != null && SeedOptions.OverwriteExisting)
                        {
                            remove(entity);
                            item.Entity = null;                            
                        }

                        if (item.Entity == null)
                        {
                            item.Entity = entity;
                            DbContext.Set<TEntity>().Add(entity);
                            DbContext.SaveChanges();
                            item.Status = SeedDataResultItemStatusType.Success;
                        }
                        else
                        {
                            item.Status = SeedDataResultItemStatusType.Exists;
                        }
                    }

                    if (SeedOptions.Delete)
                    {
                        if (DeleteFile(fileName))
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded. File delete succeeded.", typeof(TEntity).Name, fileName);
                        }
                        else
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded. File delete failed.", typeof(TEntity).Name, fileName);
                        }
                    }
                    else
                    {
                        if (result.Items.All(x => x.Status == SeedDataResultItemStatusType.Exists))
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' skipped, data already exists.", typeof(TEntity).Name, fileName);
                        }
                        else 
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded.", typeof(TEntity).Name, fileName);
                        }

                        
                    }
                }
                else
                {
                    result.Message = string.Format("Seeding '{0}' collection skipped. No items found.", typeof(TEntity).Name, fileName);
                }
            }
            catch (Exception ex)
            {
                result.Message = string.Format("Seeding '{0}' collection from '{1}' failed.", typeof(TEntity).Name, fileName);
                result.Exception = ex;
            }

            return result;
        }
    }
}

