using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class WarehouseTranafer_project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransferDetail_Project_TransferFromId",
                table: "WarehouseTransferDetail");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransferDetail_TransferFromId",
                table: "WarehouseTransferDetail");

            migrationBuilder.DropColumn(
                name: "TransferFromId",
                table: "WarehouseTransferDetail");

            migrationBuilder.AddColumn<long>(
                name: "TransferFromId",
                table: "WarehouseTransfer",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransfer_TransferFromId",
                table: "WarehouseTransfer",
                column: "TransferFromId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransfer_Project_TransferFromId",
                table: "WarehouseTransfer",
                column: "TransferFromId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseTransfer_Project_TransferFromId",
                table: "WarehouseTransfer");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseTransfer_TransferFromId",
                table: "WarehouseTransfer");

            migrationBuilder.DropColumn(
                name: "TransferFromId",
                table: "WarehouseTransfer");

            migrationBuilder.AddColumn<long>(
                name: "TransferFromId",
                table: "WarehouseTransferDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseTransferDetail_TransferFromId",
                table: "WarehouseTransferDetail",
                column: "TransferFromId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseTransferDetail_Project_TransferFromId",
                table: "WarehouseTransferDetail",
                column: "TransferFromId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
