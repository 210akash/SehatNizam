using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ProcessApprovBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "PurchaseDemand",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "PurchaseDemand",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "PurchaseDemand",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "PurchaseDemand",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "IndentRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "IndentRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "IndentRequest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "IndentRequest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_ApprovedById",
                table: "PurchaseDemand",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_ProcessedById",
                table: "PurchaseDemand",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_IndentRequest_ApprovedById",
                table: "IndentRequest",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_IndentRequest_ProcessedById",
                table: "IndentRequest",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IndentRequest_AspNetUsers_ApprovedById",
                table: "IndentRequest",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IndentRequest_AspNetUsers_ProcessedById",
                table: "IndentRequest",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_AspNetUsers_ApprovedById",
                table: "PurchaseDemand",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_AspNetUsers_ProcessedById",
                table: "PurchaseDemand",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndentRequest_AspNetUsers_ApprovedById",
                table: "IndentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_IndentRequest_AspNetUsers_ProcessedById",
                table: "IndentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_AspNetUsers_ApprovedById",
                table: "PurchaseDemand");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_AspNetUsers_ProcessedById",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_ApprovedById",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_ProcessedById",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_IndentRequest_ApprovedById",
                table: "IndentRequest");

            migrationBuilder.DropIndex(
                name: "IX_IndentRequest_ProcessedById",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "IndentRequest");
        }
    }
}
