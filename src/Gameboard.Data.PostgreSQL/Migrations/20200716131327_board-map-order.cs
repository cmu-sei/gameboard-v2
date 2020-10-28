// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using Microsoft.EntityFrameworkCore.Migrations;

namespace Gameboard.Data.PostgreSQL.Migrations
{
    public partial class boardmaporder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Maps");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Maps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Boards");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Boards",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Maps");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Maps",
                nullable: false,
                defaultValue: false);

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Boards");

            migrationBuilder.AddColumn<bool>(
                name: "Order",
                table: "Boards",
                nullable: true,
                defaultValue: null);
        }
    }
}

