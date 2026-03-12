using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class GRNApprovals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "GRN",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "GRN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "GRN",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "GRN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRN_ApprovedById",
                table: "GRN",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_GRN_ProcessedById",
                table: "GRN",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_AspNetUsers_ApprovedById",
                table: "GRN",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GRN_AspNetUsers_ProcessedById",
                table: "GRN",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRN_AspNetUsers_ApprovedById",
                table: "GRN");

            migrationBuilder.DropForeignKey(
                name: "FK_GRN_AspNetUsers_ProcessedById",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_ApprovedById",
                table: "GRN");

            migrationBuilder.DropIndex(
                name: "IX_GRN_ProcessedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "GRN");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "GRN");
        }
    }
}
