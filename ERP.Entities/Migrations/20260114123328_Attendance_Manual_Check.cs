using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Attendance_Manual_Check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsManualIn",
                table: "UserAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManualOut",
                table: "UserAttendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ManualById",
                table: "UserAttendance",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "ShopOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAttendance_ManualById",
                table: "UserAttendance",
                column: "ManualById");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_AspNetUsers_ManualById",
                table: "UserAttendance",
                column: "ManualById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_AspNetUsers_ManualById",
                table: "UserAttendance");

            migrationBuilder.DropIndex(
                name: "IX_UserAttendance_ManualById",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "IsManualIn",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "IsManualOut",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "ManualById",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "ShopOrder");
        }
    }
}
