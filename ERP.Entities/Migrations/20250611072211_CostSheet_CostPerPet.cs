using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class CostSheet_CostPerPet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostPerPet",
                table: "CostSheet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TFillingPerPet",
                table: "CostSheet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TMaterialCost",
                table: "CostSheet",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostPerPet",
                table: "CostSheet");

            migrationBuilder.DropColumn(
                name: "TFillingPerPet",
                table: "CostSheet");

            migrationBuilder.DropColumn(
                name: "TMaterialCost",
                table: "CostSheet");
        }
    }
}
