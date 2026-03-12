using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class FixedAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedAsset",
                table: "Status");

            migrationBuilder.AddColumn<bool>(
                name: "FixedAsset",
                table: "Store",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedAsset",
                table: "Store");

            migrationBuilder.AddColumn<bool>(
                name: "FixedAsset",
                table: "Status",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
