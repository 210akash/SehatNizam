using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ComparativeStatementDemand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparativeStatementDetail_Item_ItemId",
                table: "ComparativeStatementDetail");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ComparativeStatementDetail");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ComparativeStatementDetail",
                newName: "PurchaseDemandDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_ComparativeStatementDetail_ItemId",
                table: "ComparativeStatementDetail",
                newName: "IX_ComparativeStatementDetail_PurchaseDemandDetailId");

            migrationBuilder.AlterColumn<long>(
                name: "ComparativeStatementVendorId",
                table: "PurchaseOrderDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "PurchaseDemand",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PurchaseDemandId",
                table: "ComparativeStatement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_PurchaseDemandId",
                table: "ComparativeStatement",
                column: "PurchaseDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparativeStatement_PurchaseDemand_PurchaseDemandId",
                table: "ComparativeStatement",
                column: "PurchaseDemandId",
                principalTable: "PurchaseDemand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComparativeStatementDetail_PurchaseDemandDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail",
                column: "PurchaseDemandDetailId",
                principalTable: "PurchaseDemandDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparativeStatement_PurchaseDemand_PurchaseDemandId",
                table: "ComparativeStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_ComparativeStatementDetail_PurchaseDemandDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail");

            migrationBuilder.DropIndex(
                name: "IX_ComparativeStatement_PurchaseDemandId",
                table: "ComparativeStatement");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "PurchaseDemandId",
                table: "ComparativeStatement");

            migrationBuilder.RenameColumn(
                name: "PurchaseDemandDetailId",
                table: "ComparativeStatementDetail",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ComparativeStatementDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail",
                newName: "IX_ComparativeStatementDetail_ItemId");

            migrationBuilder.AlterColumn<long>(
                name: "ComparativeStatementVendorId",
                table: "PurchaseOrderDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "ComparativeStatementDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_ComparativeStatementDetail_Item_ItemId",
                table: "ComparativeStatementDetail",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
