using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class GLAccount_Service : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountGroupId",
                table: "Service",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Service",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Service",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Service_AccountGroupId",
                table: "Service",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_AccountId",
                table: "Service",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Account_AccountId",
                table: "Service",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_AccountGroup_AccountGroupId",
                table: "Service",
                column: "AccountGroupId",
                principalTable: "AccountGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Account_AccountId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_AccountGroup_AccountGroupId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_AccountGroupId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_AccountId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Service");
        }
    }
}
