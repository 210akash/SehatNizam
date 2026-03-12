using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class UserAttendance_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Attendance",
                table: "UserAttendance",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceType",
                table: "UserAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserAttendance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "UserAttendance",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeHours",
                table: "UserAttendance",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeIn",
                table: "UserAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOut",
                table: "UserAttendance",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WorkingHours",
                table: "UserAttendance",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendance",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "AttendanceType",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "OverTimeHours",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "UserAttendance");
        }
    }
}
