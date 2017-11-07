namespace _02.SocialNetwork.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RolesToAlbumUserTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "AlbumUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AlbumUser");
        }
    }
}