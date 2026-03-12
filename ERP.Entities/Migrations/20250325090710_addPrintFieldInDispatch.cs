using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class addPrintFieldInDispatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrintById",
                table: "DispatchOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrintDate",
                table: "DispatchOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_PrintById",
                table: "DispatchOrder",
                column: "PrintById");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchOrder_AspNetUsers_PrintById",
                table: "DispatchOrder",
                column: "PrintById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchOrder_AspNetUsers_PrintById",
                table: "DispatchOrder");

            migrationBuilder.DropIndex(
                name: "IX_DispatchOrder_PrintById",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "PrintById",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "PrintDate",
                table: "DispatchOrder");
        }
    }
}
