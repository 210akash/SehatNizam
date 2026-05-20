using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AccountGroup_Employee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "AccountGroup",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId1",
                table: "AccountGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_EmployeeId1",
                table: "AccountGroup",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountGroup_AspNetUsers_EmployeeId1",
                table: "AccountGroup",
                column: "EmployeeId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountGroup_AspNetUsers_EmployeeId1",
                table: "AccountGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroup_EmployeeId1",
                table: "AccountGroup");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AccountGroup");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "AccountGroup");
        }
    }
}
