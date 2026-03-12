using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Issuance_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssuanceDetail_Project_ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.DropIndex(
                name: "IX_IssuanceDetail_ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "Issuance",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_ProjectId",
                table: "Issuance",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issuance_Project_ProjectId",
                table: "Issuance",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issuance_Project_ProjectId",
                table: "Issuance");

            migrationBuilder.DropIndex(
                name: "IX_Issuance_ProjectId",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Issuance");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "IssuanceDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_ProjectId",
                table: "IssuanceDetail",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssuanceDetail_Project_ProjectId",
                table: "IssuanceDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
