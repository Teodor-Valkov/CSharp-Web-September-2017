using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Migrations
{
    public partial class AddedOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGame_Games_GameId",
                table: "UserGame");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGame_Users_UserId",
                table: "UserGame");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGame",
                table: "UserGame");

            migrationBuilder.RenameTable(
                name: "UserGame",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_UserGame_GameId",
                table: "Orders",
                newName: "IX_Orders_GameId");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                columns: new[] { "UserId", "GameId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Games_GameId",
                table: "Orders",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Games_GameId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "UserGame");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_GameId",
                table: "UserGame",
                newName: "IX_UserGame_GameId");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Games",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGame",
                table: "UserGame",
                columns: new[] { "UserId", "GameId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserGame_Games_GameId",
                table: "UserGame",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGame_Users_UserId",
                table: "UserGame",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}