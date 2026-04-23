using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Aspnetusers_employeesalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_EmployeeId",
                table: "EmployeeSalary");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_EmployeeId",
                table: "EmployeeSalary",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_EmployeeId",
                table: "EmployeeSalary");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_EmployeeId",
                table: "EmployeeSalary",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
