// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

// <auto-generated />
using System;
using Gameboard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    [DbContext(typeof(GameboardDbContext))]
    [Migration("20191003155800_team-disabled-badges")]
    partial class teamdisabledbadges
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Gameboard.Data.Board", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<bool>("AllowPreview");

                    b.Property<bool>("IsPractice");

                    b.Property<int>("MaxConcurrentProblems");

                    b.Property<int>("MaxMinutes");

                    b.Property<int>("MaxSubmissions");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartTime");

                    b.Property<DateTime>("StopTime");

                    b.HasKey("Id");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Gameboard.Data.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("BoardId")
                        .HasMaxLength(40);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Gameboard.Data.Challenge", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<int>("CategoryId");

                    b.Property<string>("CategoryId1");

                    b.Property<string>("Description");

                    b.Property<string>("FlagStyle");

                    b.Property<int>("Points");

                    b.Property<string>("Tags");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId1");

                    b.ToTable("Challenges");
                });

            modelBuilder.Entity("Gameboard.Data.Problem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("BoardId")
                        .HasMaxLength(40);

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<DateTime?>("End");

                    b.Property<bool>("GamespaceReady");

                    b.Property<bool>("HasGamespace");

                    b.Property<int>("MaxSubmissions");

                    b.Property<long>("Score");

                    b.Property<DateTime>("Start");

                    b.Property<string>("Status");

                    b.Property<string>("TeamId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("TeamId");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Gameboard.Data.Submission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("Flag");

                    b.Property<string>("ProblemId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<int>("Status");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.HasIndex("UserId");

                    b.ToTable("Submissions");
                });

            modelBuilder.Entity("Gameboard.Data.Team", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("Badges");

                    b.Property<DateTime>("Created");

                    b.Property<string>("InviteCode");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsLocked");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Number");

                    b.Property<string>("OrganizationLogoUrl");

                    b.Property<string>("OrganizationName");

                    b.Property<string>("OrganizationalUnitLogoUrl");

                    b.Property<string>("OwnerUserId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Gameboard.Data.TeamBoard", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("BoardId")
                        .IsRequired();

                    b.Property<int?>("OverrideMaxMinutes");

                    b.Property<long>("Score");

                    b.Property<DateTime>("Start");

                    b.Property<string>("TeamId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamBoards");
                });

            modelBuilder.Entity("Gameboard.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<bool>("IsModerator");

                    b.Property<string>("Name");

                    b.Property<string>("Organization");

                    b.Property<string>("TeamId")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Gameboard.Data.Category", b =>
                {
                    b.HasOne("Gameboard.Data.Board", "Board")
                        .WithMany("Categories")
                        .HasForeignKey("BoardId");
                });

            modelBuilder.Entity("Gameboard.Data.Challenge", b =>
                {
                    b.HasOne("Gameboard.Data.Category", "Category")
                        .WithMany("Challenges")
                        .HasForeignKey("CategoryId1");
                });

            modelBuilder.Entity("Gameboard.Data.Problem", b =>
                {
                    b.HasOne("Gameboard.Data.Challenge", "Challenge")
                        .WithMany("Problems")
                        .HasForeignKey("ChallengeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Gameboard.Data.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Submission", b =>
                {
                    b.HasOne("Gameboard.Data.Problem", "Problem")
                        .WithMany("Submissions")
                        .HasForeignKey("ProblemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Gameboard.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.TeamBoard", b =>
                {
                    b.HasOne("Gameboard.Data.Board", "Board")
                        .WithMany()
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Gameboard.Data.Team", "Team")
                        .WithMany("TeamBoards")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.User", b =>
                {
                    b.HasOne("Gameboard.Data.Team", "Team")
                        .WithMany("Users")
                        .HasForeignKey("TeamId");
                });
#pragma warning restore 612, 618
        }
    }
}

