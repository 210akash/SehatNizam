using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Grn_PurchaseInv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceApprovedById",
                table: "GRN",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceApprovedDate",
                table: "GRN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "GRN",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRN_InvoiceApprovedById",
                table: "GRN",
                column: "InvoiceApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceApprovedById",
                table: "GRN",
                column: "InvoiceApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceApprovedById",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_InvoiceApprovedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceApprovedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceApprovedDate",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "GRN");
        }
    }
}
