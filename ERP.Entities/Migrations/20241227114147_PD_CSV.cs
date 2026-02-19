using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PD_CSV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparativeStatementDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail",
                column: "PurchaseDemandDetailId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparativeStatementDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_PurchaseDemandDetailId",
                table: "ComparativeStatementDetail",
                column: "PurchaseDemandDetailId");
        }
    }
}
