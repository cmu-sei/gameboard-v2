// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gameboard.Data
{
    public class GameboardDbContext : DbContext
    {
        //game
        public DbSet<Game> Games { get; set; }
        public DbSet<Board> Boards { get; set; }

        //trivia
        public DbSet<Category> Categories { get; set; }
        public DbSet<Question> Questions { get; set; }

        //map
        public DbSet<Map> Maps{ get; set; }
        public DbSet<Coordinate> Coordinates { get; set; }
        
        //player
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Problem> Problems { get; set; }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<TeamBoard> TeamBoards { get; set; }
        public DbSet<Survey> Surveys { get; set; }

        DbContextOptions Options { get; }

        public GameboardDbContext(DbContextOptions options)
            : base(options)
        {
            Options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Survey>().HasKey(x => new { x.ChallengeId, x.UserId });
            builder.Entity<TeamBoard>().HasKey(x => new { x.TeamId, x.BoardId });
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            PreProcesses();
            return base.SaveChanges();
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PreProcesses();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            PreProcesses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            PreProcesses();
            return base.SaveChangesAsync(cancellationToken);
        }

        void PreProcesses()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                ProcessIAudit(entry);
                ProcessISlug(entry);
            }
        }

        private static void ProcessISlug(EntityEntry entry)
        {
            if (entry.Entity is ISlug slug)
            {
                var urlString = slug.Name.ToUrlString();

                if (string.IsNullOrWhiteSpace(slug.Slug) || slug.Slug != urlString)
                {
                    slug.Slug = urlString;
                }
            }
        }

        private static void ProcessIAudit(EntityEntry entry)
        {
            if (entry.Entity is IAudit audit)
            {
                if (entry.State == EntityState.Added)
                {
                    audit.Created = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    audit.Updated = DateTime.UtcNow;
                }
            }
        }
    }
}

