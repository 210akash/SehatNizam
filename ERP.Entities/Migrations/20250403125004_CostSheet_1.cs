using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class CostSheet_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ItemId",
                table: "CostSheet",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CostSheet_ItemId",
                table: "CostSheet",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostSheet_Item_ItemId",
                table: "CostSheet",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostSheet_Item_ItemId",
                table: "CostSheet");

            migrationBuilder.DropIndex(
                name: "IX_CostSheet_ItemId",
                table: "CostSheet");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "CostSheet");
        }
    }
}
