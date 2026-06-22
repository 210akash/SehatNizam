using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ServiceAccount_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "ServiceAccount",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_ProjectId",
                table: "ServiceAccount",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAccount_Project_ProjectId",
                table: "ServiceAccount",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAccount_Project_ProjectId",
                table: "ServiceAccount");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAccount_ProjectId",
                table: "ServiceAccount");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ServiceAccount");
        }
    }
}
