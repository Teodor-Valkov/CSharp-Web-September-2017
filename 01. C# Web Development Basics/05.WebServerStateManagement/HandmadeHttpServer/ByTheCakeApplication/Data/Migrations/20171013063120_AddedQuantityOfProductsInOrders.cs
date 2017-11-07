namespace HandmadeHttpServer.ByTheCakeApplication.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddedQuantityOfProductsInOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderProduct");
        }
    }
}