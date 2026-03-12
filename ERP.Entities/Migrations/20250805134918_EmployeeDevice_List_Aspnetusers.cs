using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeDevice_List_Aspnetusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDevice_AspNetUsers_EmployeeId",
                table: "EmployeeDevice");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDevice_AspNetUsers_EmployeeId",
                table: "EmployeeDevice",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDevice_AspNetUsers_EmployeeId",
                table: "EmployeeDevice");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDevice_AspNetUsers_EmployeeId",
                table: "EmployeeDevice",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
