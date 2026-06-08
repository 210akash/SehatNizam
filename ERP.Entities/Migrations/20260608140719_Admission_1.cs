using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Admission_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "Ward",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Room",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ward_ProjectId",
                table: "Ward",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ward_Project_ProjectId",
                table: "Ward",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ward_Project_ProjectId",
                table: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_Ward_ProjectId",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Ward");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Room");
        }
    }
}
