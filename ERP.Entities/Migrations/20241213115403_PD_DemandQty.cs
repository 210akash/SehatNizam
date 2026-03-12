using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PD_DemandQty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Required",
                table: "PurchaseDemandDetail",
                newName: "DemandQty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DemandQty",
                table: "PurchaseDemandDetail",
                newName: "Required");
        }
    }
}
