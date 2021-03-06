﻿namespace FitStore.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddedIsRestrictedColumnToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRestricted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRestricted",
                table: "AspNetUsers");
        }
    }
}