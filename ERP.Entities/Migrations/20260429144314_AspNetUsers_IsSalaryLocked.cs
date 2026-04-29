using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AspNetUsers_IsSalaryLocked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "EmployeeSalary");

            migrationBuilder.AddColumn<bool>(
                name: "IsSalaryLocked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSalaryLocked",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "EmployeeSalary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
