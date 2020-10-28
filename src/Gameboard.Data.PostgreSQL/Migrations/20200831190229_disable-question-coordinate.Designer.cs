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
    [Migration("20200831190229_disable-question-coordinate")]
    partial class disablequestioncoordinate
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
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Badges");

                    b.Property<int>("BoardType");

                    b.Property<string>("GameId")
                        .IsRequired();

                    b.Property<bool>("IsPractice");

                    b.Property<bool>("IsPreviewAllowed");

                    b.Property<bool>("IsResetAllowed");

                    b.Property<bool>("IsTitleVisible");

                    b.Property<int>("MaxMinutes");

                    b.Property<int>("MaxSubmissions");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<int>("Order");

                    b.Property<string>("RequiredBadges");

                    b.Property<string>("StartText");

                    b.Property<DateTime?>("StartTime");

                    b.Property<DateTime?>("StopTime");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("Gameboard.Data.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BoardId")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("Order");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Gameboard.Data.Coordinate", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActionType");

                    b.Property<string>("ActionValue");

                    b.Property<bool>("IsDisabled");

                    b.Property<string>("MapId")
                        .IsRequired();

                    b.Property<string>("Name");

                    b.Property<int>("Points");

                    b.Property<double>("Radius");

                    b.Property<double>("X");

                    b.Property<double>("Y");

                    b.HasKey("Id");

                    b.HasIndex("MapId");

                    b.ToTable("Coordinates");
                });

            modelBuilder.Entity("Gameboard.Data.Game", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<DateTime?>("EnrollmentEndsAt");

                    b.Property<bool>("IsLocked");

                    b.Property<int>("MaxConcurrentProblems");

                    b.Property<int>("MaxTeamSize");

                    b.Property<int>("MinTeamSize");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("StartTime");

                    b.Property<DateTime?>("StopTime");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Gameboard.Data.Map", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BoardId")
                        .IsRequired();

                    b.Property<string>("ImageUrl")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.HasKey("Id");

                    b.HasIndex("BoardId");

                    b.ToTable("Maps");
                });

            modelBuilder.Entity("Gameboard.Data.Problem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<string>("BoardId")
                        .HasMaxLength(40);

                    b.Property<string>("ChallengeLinkId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<DateTime?>("End");

                    b.Property<bool>("GamespaceReady");

                    b.Property<string>("GamespaceText");

                    b.Property<bool>("HasGamespace");

                    b.Property<int>("MaxSubmissions");

                    b.Property<double>("Score");

                    b.Property<DateTime>("Start");

                    b.Property<string>("Status");

                    b.Property<string>("TeamId")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Problems");
                });

            modelBuilder.Entity("Gameboard.Data.Question", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CategoryId")
                        .IsRequired();

                    b.Property<bool>("IsDisabled");

                    b.Property<int?>("Order");

                    b.Property<int>("Points");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("Gameboard.Data.Submission", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

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

            modelBuilder.Entity("Gameboard.Data.Survey", b =>
                {
                    b.Property<string>("ChallengeId");

                    b.Property<string>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Data");

                    b.HasKey("ChallengeId", "UserId");

                    b.ToTable("Surveys");
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

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Gameboard.Data.TeamBoard", b =>
                {
                    b.Property<string>("TeamId");

                    b.Property<string>("BoardId");

                    b.Property<int?>("OverrideMaxMinutes");

                    b.Property<double>("Score");

                    b.Property<DateTime>("Start");

                    b.HasKey("TeamId", "BoardId");

                    b.ToTable("TeamBoards");
                });

            modelBuilder.Entity("Gameboard.Data.Token", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<int?>("Index");

                    b.Property<string>("Label");

                    b.Property<int>("Percent");

                    b.Property<string>("ProblemId");

                    b.Property<int>("Status");

                    b.Property<string>("SubmissionId");

                    b.Property<DateTime?>("Timestamp");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ProblemId");

                    b.HasIndex("SubmissionId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Gameboard.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(40);

                    b.Property<bool>("IsChallengeDeveloper");

                    b.Property<bool>("IsGameDesigner");

                    b.Property<bool>("IsModerator");

                    b.Property<bool>("IsObserver");

                    b.Property<string>("Name");

                    b.Property<string>("Organization");

                    b.Property<string>("Survey");

                    b.Property<string>("TeamId")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Gameboard.Data.Board", b =>
                {
                    b.HasOne("Gameboard.Data.Game", "Game")
                        .WithMany("Boards")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Category", b =>
                {
                    b.HasOne("Gameboard.Data.Board", "Board")
                        .WithMany("Categories")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Coordinate", b =>
                {
                    b.HasOne("Gameboard.Data.Map", "Map")
                        .WithMany("Coordinates")
                        .HasForeignKey("MapId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Gameboard.Data.ChallengeLink", "ChallengeLink", b1 =>
                        {
                            b1.Property<string>("Id")
                                .HasMaxLength(40);

                            b1.Property<string>("Slug");

                            b1.HasKey("Id");

                            b1.ToTable("Coordinates");

                            b1.HasOne("Gameboard.Data.Coordinate")
                                .WithOne("ChallengeLink")
                                .HasForeignKey("Gameboard.Data.ChallengeLink", "Id")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("Gameboard.Data.Map", b =>
                {
                    b.HasOne("Gameboard.Data.Board", "Board")
                        .WithMany("Maps")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Problem", b =>
                {
                    b.HasOne("Gameboard.Data.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Question", b =>
                {
                    b.HasOne("Gameboard.Data.Category", "Category")
                        .WithMany("Questions")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Gameboard.Data.ChallengeLink", "ChallengeLink", b1 =>
                        {
                            b1.Property<string>("Id")
                                .HasMaxLength(40);

                            b1.Property<string>("Slug");

                            b1.HasKey("Id");

                            b1.ToTable("Questions");

                            b1.HasOne("Gameboard.Data.Question")
                                .WithOne("ChallengeLink")
                                .HasForeignKey("Gameboard.Data.ChallengeLink", "Id")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
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
                    b.HasOne("Gameboard.Data.Team", "Team")
                        .WithMany("TeamBoards")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Gameboard.Data.Token", b =>
                {
                    b.HasOne("Gameboard.Data.Problem", "Problem")
                        .WithMany("Tokens")
                        .HasForeignKey("ProblemId");

                    b.HasOne("Gameboard.Data.Submission", "Submission")
                        .WithMany("Tokens")
                        .HasForeignKey("SubmissionId");
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

