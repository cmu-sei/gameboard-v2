// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    public partial class games : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallengeSurveys");

            migrationBuilder.RenameColumn(
                name: "ChallengeId",
                table: "Problems",
                newName: "ChallengeLinkId");

            migrationBuilder.AddColumn<bool>(
                name: "IsGameDesigner",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    MaxTeamSize = table.Column<int>(nullable: false),
                    MinTeamSize = table.Column<int>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: true),
                    StopTime = table.Column<DateTime>(nullable: true),
                    EnrollmentEndsAt = table.Column<DateTime>(nullable: true),
                    MaxConcurrentProblems = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    ChallengeId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => new { x.ChallengeId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    GameId = table.Column<string>(nullable: true),
                    Badges = table.Column<string>(nullable: true),
                    RequiredBadges = table.Column<string>(nullable: true),
                    MaxSubmissions = table.Column<int>(nullable: false),
                    MaxMinutes = table.Column<int>(nullable: false),
                    IsPreviewAllowed = table.Column<bool>(nullable: false),
                    IsPractice = table.Column<bool>(nullable: false),
                    BoardType = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BoardId = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: false),
                    BoardId = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maps_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false),
                    ChallengeLink_Slug = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    X = table.Column<double>(nullable: false),
                    Y = table.Column<double>(nullable: false),
                    Radius = table.Column<double>(nullable: false),
                    MapId = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    ActionValue = table.Column<string>(nullable: true),
                    ChallengeLink_Slug = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coordinates_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_GameId",
                table: "Boards",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BoardId",
                table: "Categories",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_MapId",
                table: "Coordinates",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_BoardId",
                table: "Maps",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CategoryId",
                table: "Questions",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinates");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropColumn(
                name: "IsGameDesigner",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ChallengeLinkId",
                table: "Problems",
                newName: "ChallengeId");

            migrationBuilder.CreateTable(
                name: "ChallengeSurveys",
                columns: table => new
                {
                    ChallengeId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeSurveys", x => new { x.ChallengeId, x.UserId });
                });
        }
    }
}

