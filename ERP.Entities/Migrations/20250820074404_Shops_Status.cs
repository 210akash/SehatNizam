using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Shops_Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "Shops",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shops_StatusId",
                table: "Shops",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Status_StatusId",
                table: "Shops",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Status_StatusId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_StatusId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Shops");
        }
    }
}
