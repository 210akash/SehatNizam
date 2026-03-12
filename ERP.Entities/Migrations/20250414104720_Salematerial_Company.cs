using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Salematerial_Company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "SaleMaterial",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterial_CompanyId",
                table: "SaleMaterial",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleMaterial_Company_CompanyId",
                table: "SaleMaterial",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleMaterial_Company_CompanyId",
                table: "SaleMaterial");

            migrationBuilder.DropIndex(
                name: "IX_SaleMaterial_CompanyId",
                table: "SaleMaterial");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SaleMaterial");
        }
    }
}
