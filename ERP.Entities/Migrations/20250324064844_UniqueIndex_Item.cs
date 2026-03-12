using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class UniqueIndex_Item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CompanyId_Code",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_ItemType_CompanyId_Code",
                table: "ItemType");

            migrationBuilder.DropIndex(
                name: "IX_Item_CompanyId_Code",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Category_CompanyId_Code",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CompanyId_Code_IsActive",
                table: "SubCategory",
                columns: new[] { "CompanyId", "Code", "IsActive" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemType_CompanyId_Code_IsActive",
                table: "ItemType",
                columns: new[] { "CompanyId", "Code", "IsActive" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CompanyId_Code_IsActive",
                table: "Item",
                columns: new[] { "CompanyId", "Code", "IsActive" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId_Code_IsActive",
                table: "Category",
                columns: new[] { "CompanyId", "Code", "IsActive" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CompanyId_Code_IsActive",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_ItemType_CompanyId_Code_IsActive",
                table: "ItemType");

            migrationBuilder.DropIndex(
                name: "IX_Item_CompanyId_Code_IsActive",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Category_CompanyId_Code_IsActive",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CompanyId_Code",
                table: "SubCategory",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemType_CompanyId_Code",
                table: "ItemType",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CompanyId_Code",
                table: "Item",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId_Code",
                table: "Category",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
