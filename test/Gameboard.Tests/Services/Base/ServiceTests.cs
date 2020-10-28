// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.Repositories;
using Gameboard.Security;
using Gameboard.Services;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stack.Validation.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Tests
{
    public abstract class ServiceTests
    {
        public ServiceTests()
        {
            Initialize();
        }

        protected ILogger _logger;
        public DbContextOptions<GameboardDbContext> _dbContextOptions;

        protected TestContext CreateTestContext(User user = null)
        {
            var dbContext = new GameboardDbContext(_dbContextOptions);
            var identityResolver = new TestIdentityResolver(user);

            if (user != null)
            {
                if (!dbContext.Users.Any(u => u.Id == user.Id))
                {
                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }
            }

            return new TestContext(dbContext, _logger,
                new StrictValidationHandler(dbContext, identityResolver, new OrganizationOptions() { }),
                new TestGameCache(), new TestChallengeSpecCache())
            {
                User = user
            };
        }

        protected async Task<User> CreateUser(TestContext context, string name = null, bool isModerator = false)
        {
            name = name ?? Guid.NewGuid().ToString();

            var user = new User { Name = name, IsModerator = isModerator, Organization = OrganizationName };

            return await CreateUser(context, user);
        }

        protected async Task<User> CreateUser(TestContext context, User user)
        {
            await context.DbContext.Users.AddAsync(user);
            await context.DbContext.SaveChangesAsync();

            return user;
        }

        public readonly string OrganizationName = "OrgName";

        protected TeamCreate GetTeamCreate(string name = "Create Team")
        {
            return new TeamCreate
            {
                Name = name,
                OrganizationName = OrganizationName
            };
        }

        protected User GetUser()
        {
            var id = Guid.NewGuid().ToString();

            return new User { Id = id, Name = id, Organization = OrganizationName };
        }

        public TeamService GetTeamService(TestContext context)
        {
            var identityResolver = context.GetIdentityResolver();

            return new TeamService(
                identityResolver,
                new TeamRepository(context.DbContext, new TeamPermissionMediator(identityResolver)),
                new ProblemRepository(context.DbContext),
                context.GetMapper(),
                context.GetValidationHandler(),
                context.GetCache<User>(),
                new LeaderboardOptions { },
                new TestGameEngineService(),
                context.GetGameFactory());
        }

        public BoardService GetBoardService(TestContext context)
        {
            return new BoardService(
                context.GetIdentityResolver(),
                context.GetMapper(),
                context.GetValidationHandler(),                
                context.DbContext,
                new TestGameEngineService(),
                context.GetGameFactory(),
                new LeaderboardOptions { });
        }

        public void Initialize()
        {
            _logger = new TestLogger();

            _dbContextOptions = new DbContextOptionsBuilder<GameboardDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = CreateTestContext())
            {
                ctx.DbContext.Database.EnsureDeleted();
                ctx.DbContext.Database.EnsureCreated();
            }
        }
    }
}

