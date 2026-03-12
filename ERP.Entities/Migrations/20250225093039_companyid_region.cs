using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class companyid_region : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTerritory_AspNetUsers_UserId",
                table: "UserTerritory");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Region",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_CompanyId",
                table: "Region",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Region_Company_CompanyId",
                table: "Region",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTerritory_AspNetUsers_UserId",
                table: "UserTerritory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Region_Company_CompanyId",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTerritory_AspNetUsers_UserId",
                table: "UserTerritory");

            migrationBuilder.DropIndex(
                name: "IX_Region_CompanyId",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Region");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTerritory_AspNetUsers_UserId",
                table: "UserTerritory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
