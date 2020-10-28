// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.SqlServer.Migrations
{
    public partial class multiflagupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Flag",
                table: "Submissions",
                newName: "Tokens");

            migrationBuilder.CreateTable(
                name: "SubmissionResult",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Percent = table.Column<int>(nullable: false),
                    FlagStatus = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    SubmissionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionResult_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionResult_SubmissionId",
                table: "SubmissionResult",
                column: "SubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionResult");

            migrationBuilder.RenameColumn(
                name: "Tokens",
                table: "Submissions",
                newName: "Flag");
        }
    }
}

