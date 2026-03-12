using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class dispatch_basehistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "Dispatch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Dispatch",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "Dispatch",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "Dispatch",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispatch_ApprovedById",
                table: "Dispatch",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatch_ProcessedById",
                table: "Dispatch",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatch_AspNetUsers_ApprovedById",
                table: "Dispatch",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatch_AspNetUsers_ProcessedById",
                table: "Dispatch",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatch_AspNetUsers_ApprovedById",
                table: "Dispatch");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispatch_AspNetUsers_ProcessedById",
                table: "Dispatch");

            migrationBuilder.DropIndex(
                name: "IX_Dispatch_ApprovedById",
                table: "Dispatch");

            migrationBuilder.DropIndex(
                name: "IX_Dispatch_ProcessedById",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "Dispatch");
        }
    }
}
