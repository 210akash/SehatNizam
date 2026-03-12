using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Issuance_Account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Issuance",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_AccountId",
                table: "Issuance",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issuance_Account_AccountId",
                table: "Issuance",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issuance_Account_AccountId",
                table: "Issuance");

            migrationBuilder.DropIndex(
                name: "IX_Issuance_AccountId",
                table: "Issuance");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Issuance");
        }
    }
}
