using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class leavetypedetailedit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.RenameColumn(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                newName: "EmployeeGroupLeaveTypeDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLeave_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                newName: "IX_EmployeeLeave_EmployeeGroupLeaveTypeDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveTypeDetailId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeDetailId",
                principalTable: "EmployeeGroupLeaveTypeDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveTypeDetailId",
                table: "EmployeeLeave");

            migrationBuilder.RenameColumn(
                name: "EmployeeGroupLeaveTypeDetailId",
                table: "EmployeeLeave",
                newName: "EmployeeGroupLeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLeave_EmployeeGroupLeaveTypeDetailId",
                table: "EmployeeLeave",
                newName: "IX_EmployeeLeave_EmployeeGroupLeaveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
