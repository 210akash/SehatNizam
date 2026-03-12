using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AddDepartmentInRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "AspNetRoles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_DepartmentId",
                table: "AspNetRoles",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_Department_DepartmentId",
                table: "AspNetRoles",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_Department_DepartmentId",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_DepartmentId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetRoles");
        }
    }
}
