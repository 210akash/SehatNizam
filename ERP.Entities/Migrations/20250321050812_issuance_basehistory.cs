using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class issuance_basehistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "Issuance",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Issuance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "Issuance",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "Issuance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_ApprovedById",
                table: "Issuance",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_ProcessedById",
                table: "Issuance",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Issuance_AspNetUsers_ApprovedById",
                table: "Issuance",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Issuance_AspNetUsers_ProcessedById",
                table: "Issuance",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issuance_AspNetUsers_ApprovedById",
                table: "Issuance");

            migrationBuilder.DropForeignKey(
                name: "FK_Issuance_AspNetUsers_ProcessedById",
                table: "Issuance");

            migrationBuilder.DropIndex(
                name: "IX_Issuance_ApprovedById",
                table: "Issuance");

            migrationBuilder.DropIndex(
                name: "IX_Issuance_ProcessedById",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "Issuance");
        }
    }
}
