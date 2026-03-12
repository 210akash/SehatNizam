using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class WarehouseTranafser_Costsheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CostSheetId",
                table: "WarehouseTransferDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferDetail_CostSheetId",
                table: "WarehouseTransferDetail",
                column: "CostSheetId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransferDetail_CostSheet_CostSheetId",
                table: "WarehouseTransferDetail",
                column: "CostSheetId",
                principalTable: "CostSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransferDetail_CostSheet_CostSheetId",
                table: "WarehouseTransferDetail");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransferDetail_CostSheetId",
                table: "WarehouseTransferDetail");

            migrationBuilder.DropColumn(
                name: "CostSheetId",
                table: "WarehouseTransferDetail");
        }
    }
}
