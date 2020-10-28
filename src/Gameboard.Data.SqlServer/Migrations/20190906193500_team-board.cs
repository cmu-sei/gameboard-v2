// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.SqlServer.Migrations
{
    public partial class teamboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamBoards",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    TeamId = table.Column<string>(nullable: false),
                    BoardId = table.Column<string>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    OverrideMaxMinutes = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamBoards_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamBoards_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoards_BoardId",
                table: "TeamBoards",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoards_TeamId",
                table: "TeamBoards",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamBoards");
        }
    }
}

