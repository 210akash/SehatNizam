using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class retailorder_MODIFY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RetailOrder_AspNetUsers_DSFId",
                table: "RetailOrder");

            migrationBuilder.DropIndex(
                name: "IX_RetailOrder_DSFId",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "RetailOrderProcess");

            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "RetailOrderProcess");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "RetailOrderProcess");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "RetailOrderProcess");

            migrationBuilder.DropColumn(
                name: "BillingAmount",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "DSFId",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "IsPartial",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "OnlineTransfer",
                table: "RetailOrder");

            migrationBuilder.RenameColumn(
                name: "TransferMode",
                table: "RetailOrder",
                newName: "Reference");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "RetailOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "RetailOrder",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "RetailOrder");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "RetailOrder");

            migrationBuilder.RenameColumn(
                name: "Reference",
                table: "RetailOrder",
                newName: "TransferMode");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "RetailOrderProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "RetailOrderProcess",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "RetailOrderProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "RetailOrderProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BillingAmount",
                table: "RetailOrder",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cash",
                table: "RetailOrder",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "RetailOrder",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DSFId",
                table: "RetailOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPartial",
                table: "RetailOrder",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OnlineTransfer",
                table: "RetailOrder",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_DSFId",
                table: "RetailOrder",
                column: "DSFId");

            migrationBuilder.AddForeignKey(
                name: "FK_RetailOrder_AspNetUsers_DSFId",
                table: "RetailOrder",
                column: "DSFId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
