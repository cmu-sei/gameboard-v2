// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Gameboard.Data;
using Gameboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using Stack.Http.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Gameboard.Tests
{
    [Collection("AutoMapper")]
    public class TeamServiceTests : ServiceTests
    {
        [Fact]
        public async Task Create_CompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                Assert.NotNull(created);
                Assert.Equal("Create Team", created.Name);
            }
        }

        [Fact]
        public async Task Create_FailsOnUserHasNoTeam()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.Create(new TeamCreate { Name = "Create Another Team" })
                ).Wait();

            }
        }

        [Fact]
        public async Task Create_FailsOnTeamNameIsAvailable()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);

                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityDuplicateException>(async () =>
                    await teamService.Create(new TeamCreate { Name = "Create Team" })
                ).Wait();
            }
        }

        [Fact]
        public async Task Update_CompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                Assert.NotNull(created);
                Assert.Equal("Create Team", created.Name);

                var updated = await teamService.Update(new TeamUpdate { Id = created.Id, Name = "Update Team" });

                Assert.NotNull(updated);
                Assert.Equal("Update Team", updated.Name);

                Assert.NotEqual("Create Team", updated.Name);
            }
        }

        [Fact]
        public async Task Update_FailsOnUserIsTeamOwner()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await teamService.Update(new TeamUpdate { Id = created.Id, Name = "Update Team" })
                ).Wait();
            }
        }

        [Fact]
        public async Task Update_FailsOnUserIsModerator()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await teamService.Update(new TeamUpdate { Id = created.Id, Name = "Update Team" })
                ).Wait();
            }
        }

        [Fact]
        public async Task Update_FailsOnTeamNameIsValid()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.Update(new TeamUpdate { Id = created.Id, Name = "                 " })
                ).Wait();
            }
        }

        [Fact]
        public async Task AddUserToTeam_CompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));
            }
        }

        /// <summary>
        /// in the scenario where a user creates a team and then joins another team
        /// we need to ensure that existing members are removed and the previous
        /// team is deleted
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddUserToTeam_TeamOwnerJoinsDifferentTeamCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                // create new team
                var teamService = GetTeamService(context);
                var orphanOwnerId = context.User.Id;

                var created = await teamService.Create(GetTeamCreate("About To Be Orphaned"));
                Assert.NotNull(created);

                var orphanTeamId = created.Id;

                // invite new user
                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });                
                var abandonedUser = await CreateUser(context);

                context.User = abandonedUser;
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));

                // new user creates new team
                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                created = await teamService.Create(GetTeamCreate("New Team"));
                Assert.NotNull(created);

                // new user invites orphan owner to join team
                code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await context.DbContext.Users.SingleOrDefaultAsync(u => u.Id == orphanOwnerId);
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));

                // orphan owner original team does not exist
                Assert.False(context.DbContext.Teams.Any(t => t.Id == orphanTeamId));

                // orphan owner original team members are disassociated with team
                Assert.Null(context.DbContext.Users.Single(u => u.Id == abandonedUser.Id).TeamId);
            }
        }

        [Fact]
        public async Task AddUserToTeam_FailsOnMaxTeamSizeIsValid()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var game = context.GetGameFactory().GetGame();

                game.MaxTeamSize = 1;

                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task AddUserToTeam_FailsOnTeamInviteCodeIsValid()
        {
            using (var context = CreateTestContext(GetUser()))
            {                
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                    await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = "WRONG CODE", TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task AddUserToTeam_FailsOnEnrollmentEndsAt()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await CreateUser(context);

                var game = context.GetGameFactory().GetGame();

                game.EnrollmentEndsAt = DateTime.UtcNow.AddSeconds(-1);

                teamService = GetTeamService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task AddUserToTeam_FailsOnUserHasOrganization()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                context.User = await CreateUser(context, new User { Name = "No Org" });
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task RemoveUserFromTeam_CompletesSuccessfully()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                var member = await CreateUser(context);
                context.User = member;
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));

                context.User = owner;
                teamService = GetTeamService(context);

                Assert.True(await teamService.RemoveUserFromTeam(new TeamUserDelete { TeamId = created.Id, UserId = member.Id }));
            }
        }

        [Fact]
        public async Task RemoveUserFromTeam_FailsOnUserIsTeamOwner()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                var member = await CreateUser(context);
                context.User = member;
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await teamService.RemoveUserFromTeam(new TeamUserDelete { TeamId = created.Id, UserId = owner.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task LeaveTeam_CompletesSuccessfully()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));
                Assert.NotNull(created);

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                var member = await CreateUser(context);
                context.User = member;
                teamService = GetTeamService(context);

                Assert.True(await teamService.AddUserToTeam(new TeamUserUpdate { InviteCode = code, TeamId = created.Id }));

                Assert.True(await teamService.LeaveTeam(new TeamUserLeave { TeamId = created.Id }));
            }
        }

        [Fact]
        public async Task LeaveTeam_FailsOnUserIsTeamMember()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.LeaveTeam(new TeamUserLeave { TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task Lock_CompletesSuccessfully()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                Assert.NotNull(await teamService.Lock(new TeamLock { TeamId = created.Id }));
            }
        }

        [Fact]
        public async Task Lock_FailsOnUserIsTeamOwner()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await teamService.Lock(new TeamLock { TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task Lock_FailsOnTeamIsUnlocked()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                await teamService.Lock(new TeamLock { TeamId = created.Id });

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await teamService.Lock(new TeamLock { TeamId = created.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task GenerateInviteCode_CompletesSuccessfully()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                var code = await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id });

                Assert.NotNull(code);
            }
        }

        [Fact]
        public async Task GenerateInviteCode_FailsOnUserIsTeamOwner()
        {
            var id = Guid.NewGuid().ToString();
            var owner = new User { Id = id, Name = id, Organization = OrganizationName };

            using (var context = CreateTestContext(owner))
            {
                var teamService = GetTeamService(context);

                var created = await teamService.Create(GetTeamCreate("Create Team"));

                context.User = await CreateUser(context);
                teamService = GetTeamService(context);

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await teamService.GenerateInviteCode(new TeamInviteCode { TeamId = created.Id })
                ).Wait();
            }
        }
    }
}

