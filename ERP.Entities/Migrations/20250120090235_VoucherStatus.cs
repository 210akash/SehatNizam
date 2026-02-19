using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class VoucherStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Narration",
                table: "TransactionDetail");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "Transaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Transaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Transaction",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "Transaction",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "Transaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "Transaction",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ApprovedById",
                table: "Transaction",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ProcessedById",
                table: "Transaction",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_StatusId",
                table: "Transaction",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_AspNetUsers_ApprovedById",
                table: "Transaction",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_AspNetUsers_ProcessedById",
                table: "Transaction",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Status_StatusId",
                table: "Transaction",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_AspNetUsers_ApprovedById",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_AspNetUsers_ProcessedById",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Status_StatusId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ApprovedById",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_ProcessedById",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_StatusId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Transaction");

            migrationBuilder.AddColumn<string>(
                name: "Narration",
                table: "TransactionDetail",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
