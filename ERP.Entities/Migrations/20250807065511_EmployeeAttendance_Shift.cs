using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeAttendance_Shift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeShiftId",
                table: "UserAttendance",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShiftId",
                table: "UserAttendance",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAttendance_EmployeeShiftId",
                table: "UserAttendance",
                column: "EmployeeShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_EmployeeShift_EmployeeShiftId",
                table: "UserAttendance",
                column: "EmployeeShiftId",
                principalTable: "EmployeeShift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_EmployeeShift_EmployeeShiftId",
                table: "UserAttendance");

            migrationBuilder.DropIndex(
                name: "IX_UserAttendance_EmployeeShiftId",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "EmployeeShiftId",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "UserAttendance");
        }
    }
}
