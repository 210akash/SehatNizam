using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AccountGroup_Employee_Guid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AccountGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_EmployeeId",
                table: "AccountGroup",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountGroup_AspNetUsers_EmployeeId",
                table: "AccountGroup",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountGroup_AspNetUsers_EmployeeId",
                table: "AccountGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroup_EmployeeId",
                table: "AccountGroup");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AccountGroup");
        }
    }
}
