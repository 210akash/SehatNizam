using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeShift_IsDualDate_EmployeeSalary_IsLocked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDualDate",
                table: "EmployeeShift",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "EmployeeSalary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDualDate",
                table: "EmployeeShift");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "EmployeeSalary");
        }
    }
}
