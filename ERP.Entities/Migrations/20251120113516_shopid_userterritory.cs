using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class shopid_userterritory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ShopId",
                table: "UserTerritory",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTerritory_ShopId",
                table: "UserTerritory",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTerritory_Shops_ShopId",
                table: "UserTerritory",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTerritory_Shops_ShopId",
                table: "UserTerritory");

            migrationBuilder.DropIndex(
                name: "IX_UserTerritory_ShopId",
                table: "UserTerritory");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "UserTerritory");
        }
    }
}
