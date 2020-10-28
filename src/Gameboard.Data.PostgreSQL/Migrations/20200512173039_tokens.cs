// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    public partial class tokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionResult");

            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "Submissions");

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Percent = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    Index = table.Column<int>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    SubmissionId = table.Column<string>(nullable: true),
                    ProblemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Token_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Token_ProblemId",
                table: "Token",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_SubmissionId",
                table: "Token",
                column: "SubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.AddColumn<string>(
                name: "Tokens",
                table: "Submissions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubmissionResult",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 40, nullable: false),
                    FlagStatus = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false),
                    SubmissionId = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    Token = table.Column<string>(nullable: true)
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
    }
}

