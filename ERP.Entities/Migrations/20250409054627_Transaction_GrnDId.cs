using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Transaction_GrnDId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GRNDetailId",
                table: "Transaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_GRNDetailId",
                table: "Transaction",
                column: "GRNDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_GRNDetail_GRNDetailId",
                table: "Transaction",
                column: "GRNDetailId",
                principalTable: "GRNDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_GRNDetail_GRNDetailId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_GRNDetailId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "GRNDetailId",
                table: "Transaction");
        }
    }
}
