using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class SaleMaterial_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleMaterialDetail_Project_ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.DropIndex(
                name: "IX_SaleMaterialDetail_ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleMaterial",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterial_ProjectId",
                table: "SaleMaterial",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleMaterial_Project_ProjectId",
                table: "SaleMaterial",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleMaterial_Project_ProjectId",
                table: "SaleMaterial");

            migrationBuilder.DropIndex(
                name: "IX_SaleMaterial_ProjectId",
                table: "SaleMaterial");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleMaterial");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleMaterialDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialDetail_ProjectId",
                table: "SaleMaterialDetail",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleMaterialDetail_Project_ProjectId",
                table: "SaleMaterialDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
