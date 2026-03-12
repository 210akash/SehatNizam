using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeLeave_Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.DropColumn(
                name: "EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave");

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
