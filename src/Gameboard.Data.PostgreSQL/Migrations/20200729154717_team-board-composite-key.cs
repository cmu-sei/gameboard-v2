// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    public partial class teamboardcompositekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamBoards",
                table: "TeamBoards");

            migrationBuilder.DropIndex(
                name: "IX_TeamBoards_TeamId",
                table: "TeamBoards");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TeamBoards");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamBoards",
                table: "TeamBoards",
                columns: new[] { "TeamId", "BoardId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamBoards",
                table: "TeamBoards");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "TeamBoards",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamBoards",
                table: "TeamBoards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamBoards_TeamId",
                table: "TeamBoards",
                column: "TeamId");
        }
    }
}

