// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    public partial class gameengineentityremoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Challenges_ChallengeId",
                table: "Problems");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamBoards_Boards_BoardId",
                table: "TeamBoards");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_TeamBoards_BoardId",
                table: "TeamBoards");

            migrationBuilder.DropIndex(
                name: "IX_Problems_ChallengeId",
                table: "Problems");

            migrationBuilder.AlterColumn<string>(
                name: "BoardId",
                table: "TeamBoards",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BoardId",
                table: "TeamBoards",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    AllowPreview = table.Column<bool>(nullable: false),
                    IsPractice = table.Column<bool>(nullable: false),
                    MaxConcurrentProblems = table.Column<int>(nullable: false),
                    MaxMinutes = table.Column<int>(nullable: false),
                    MaxSubmissions = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Prerequisites = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    StopTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    BoardId = table.Column<string>(maxLength: 40, nullable: true),
                    Name = table.Column<string>(nullable: true)
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
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    CategoryId1 = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FlagStyle = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_Categories_CategoryId1",
                        column: x => x.CategoryId1,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoards_BoardId",
                table: "TeamBoards",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ChallengeId",
                table: "Problems",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BoardId",
                table: "Categories",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_CategoryId1",
                table: "Challenges",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Challenges_ChallengeId",
                table: "Problems",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamBoards_Boards_BoardId",
                table: "TeamBoards",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

