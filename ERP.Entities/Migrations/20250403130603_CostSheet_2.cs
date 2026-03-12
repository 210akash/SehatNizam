using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class CostSheet_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostSheetDetail_ComparativeStatementDetail_ComparativeStatementDetailId",
                table: "CostSheetDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostSheetDetail_ComparativeStatementDetailId",
                table: "CostSheetDetail");

            migrationBuilder.DropColumn(
                name: "ComparativeStatementDetailId",
                table: "CostSheetDetail");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "CostSheet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CostSheet");

            migrationBuilder.AddColumn<long>(
                name: "ComparativeStatementDetailId",
                table: "CostSheetDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostSheetDetail_ComparativeStatementDetailId",
                table: "CostSheetDetail",
                column: "ComparativeStatementDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostSheetDetail_ComparativeStatementDetail_ComparativeStatementDetailId",
                table: "CostSheetDetail",
                column: "ComparativeStatementDetailId",
                principalTable: "ComparativeStatementDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
