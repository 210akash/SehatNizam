using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Salereturn_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "SaleReturn",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "SaleReturn",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "SaleReturn",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "SaleReturn",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_ApprovedById",
                table: "SaleReturn",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_ProcessedById",
                table: "SaleReturn",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturn_AspNetUsers_ApprovedById",
                table: "SaleReturn",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturn_AspNetUsers_ProcessedById",
                table: "SaleReturn",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturn_AspNetUsers_ApprovedById",
                table: "SaleReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturn_AspNetUsers_ProcessedById",
                table: "SaleReturn");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturn_ApprovedById",
                table: "SaleReturn");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturn_ProcessedById",
                table: "SaleReturn");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "SaleReturn");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "SaleReturn");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "SaleReturn");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "SaleReturn");
        }
    }
}
