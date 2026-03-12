using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PD_RemoveStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_Store_StoreId",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_StoreId",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PurchaseDemand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "PurchaseDemand",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_StoreId",
                table: "PurchaseDemand",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_Store_StoreId",
                table: "PurchaseDemand",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
