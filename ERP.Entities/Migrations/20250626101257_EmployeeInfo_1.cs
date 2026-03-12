using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeInfo_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_CreatedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_ModifiedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimeRate_CreatedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimeRate_ModifiedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropColumn(
                name: "CreatedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropColumn(
                name: "ModifiedById1",
                table: "EmployeeOvertimeRate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_CreatedById",
                table: "EmployeeOvertimeRate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_ModifiedById",
                table: "EmployeeOvertimeRate",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_CreatedById",
                table: "EmployeeOvertimeRate",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_ModifiedById",
                table: "EmployeeOvertimeRate",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_CreatedById",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_ModifiedById",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimeRate_CreatedById",
                table: "EmployeeOvertimeRate");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOvertimeRate_ModifiedById",
                table: "EmployeeOvertimeRate");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById1",
                table: "EmployeeOvertimeRate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById1",
                table: "EmployeeOvertimeRate",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_CreatedById1",
                table: "EmployeeOvertimeRate",
                column: "CreatedById1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_ModifiedById1",
                table: "EmployeeOvertimeRate",
                column: "ModifiedById1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_CreatedById1",
                table: "EmployeeOvertimeRate",
                column: "CreatedById1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOvertimeRate_AspNetUsers_ModifiedById1",
                table: "EmployeeOvertimeRate",
                column: "ModifiedById1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
