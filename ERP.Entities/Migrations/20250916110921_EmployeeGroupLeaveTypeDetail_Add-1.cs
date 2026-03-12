using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeGroupLeaveTypeDetail_Add1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail");

            migrationBuilder.DropColumn(
                name: "EmployeeLeaveGroupTypeId",
                table: "EmployeeGroupLeaveTypeDetail");

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail");

            migrationBuilder.AlterColumn<long>(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeLeaveGroupTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
