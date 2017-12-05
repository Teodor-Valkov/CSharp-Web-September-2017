namespace CameraBazaar.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddedCanCreateCamerasToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanCreateCameras",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateCameras",
                table: "AspNetUsers");
        }
    }
}