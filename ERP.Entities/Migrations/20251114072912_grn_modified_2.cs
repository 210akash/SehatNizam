using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class grn_modified_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InvoiceStatusId",
                table: "GRN",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRN_InvoiceStatusId",
                table: "GRN",
                column: "InvoiceStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_Status_InvoiceStatusId",
                table: "GRN",
                column: "InvoiceStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRN_Status_InvoiceStatusId",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_InvoiceStatusId",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceStatusId",
                table: "GRN");
        }
    }
}
