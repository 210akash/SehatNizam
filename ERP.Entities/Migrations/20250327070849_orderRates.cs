using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class orderRates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DistributorAmount",
                table: "DispatchOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DistributorMargin",
                table: "DispatchOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TradeMargin",
                table: "DispatchOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TradePromo",
                table: "DispatchOrder",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributorAmount",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "DistributorMargin",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "TradeMargin",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "TradePromo",
                table: "DispatchOrder");
        }
    }
}
