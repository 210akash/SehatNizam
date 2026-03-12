using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PD_IndentRequestDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                column: "IndentRequestDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemandDetail_IndentRequestDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                column: "IndentRequestDetailId",
                principalTable: "IndentRequestDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemandDetail_IndentRequestDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemandDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropColumn(
                name: "IndentRequestDetailId",
                table: "PurchaseDemandDetail");
        }
    }
}
