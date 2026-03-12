using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class IGP_project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "IGP",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IGP_ProjectId",
                table: "IGP",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_IGP_Project_ProjectId",
                table: "IGP",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IGP_Project_ProjectId",
                table: "IGP");

            migrationBuilder.DropIndex(
                name: "IX_IGP_ProjectId",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "IGP");
        }
    }
}
