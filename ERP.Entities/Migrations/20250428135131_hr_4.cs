using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class hr_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeBank_EmployeeTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeLeaveGroup_EmployeeTypeId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "EmployeeDocument",
                newName: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeBankId",
                table: "AspNetUsers",
                column: "EmployeeBankId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeLeaveGroupId",
                table: "AspNetUsers",
                column: "EmployeeLeaveGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeBank_EmployeeBankId",
                table: "AspNetUsers",
                column: "EmployeeBankId",
                principalTable: "EmployeeBank",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeLeaveGroup_EmployeeLeaveGroupId",
                table: "AspNetUsers",
                column: "EmployeeLeaveGroupId",
                principalTable: "EmployeeLeaveGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeBank_EmployeeBankId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeLeaveGroup_EmployeeLeaveGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeBankId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeLeaveGroupId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EmployeeDocument",
                newName: "ImageName");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeBank_EmployeeTypeId",
                table: "AspNetUsers",
                column: "EmployeeTypeId",
                principalTable: "EmployeeBank",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeLeaveGroup_EmployeeTypeId",
                table: "AspNetUsers",
                column: "EmployeeTypeId",
                principalTable: "EmployeeLeaveGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
