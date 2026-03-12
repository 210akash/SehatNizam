using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class DemandIndentRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IndentRequestId",
                table: "PurchaseDemand",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_IndentRequestId",
                table: "PurchaseDemand",
                column: "IndentRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_IndentRequest_IndentRequestId",
                table: "PurchaseDemand",
                column: "IndentRequestId",
                principalTable: "IndentRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_IndentRequest_IndentRequestId",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_IndentRequestId",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "IndentRequestId",
                table: "PurchaseDemand");
        }
    }
}
