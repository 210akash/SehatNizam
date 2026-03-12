using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class grn_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "GRN",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceAuditVerifiedById",
                table: "GRN",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceAuditVerifiedDate",
                table: "GRN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceProcessedById",
                table: "GRN",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceProcessedDate",
                table: "GRN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRN_InvoiceAuditVerifiedById",
                table: "GRN",
                column: "InvoiceAuditVerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_InvoiceProcessedById",
                table: "GRN",
                column: "InvoiceProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceAuditVerifiedById",
                table: "GRN",
                column: "InvoiceAuditVerifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceProcessedById",
                table: "GRN",
                column: "InvoiceProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceAuditVerifiedById",
                table: "GRN");

            migrationBuilder.DropForeignKey(
                name: "FK_GRN_AspNetUsers_InvoiceProcessedById",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_InvoiceAuditVerifiedById",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_InvoiceProcessedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceAuditVerifiedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceAuditVerifiedDate",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceProcessedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "InvoiceProcessedDate",
                table: "GRN");
        }
    }
}
