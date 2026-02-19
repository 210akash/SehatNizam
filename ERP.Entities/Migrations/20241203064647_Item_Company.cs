using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Item_Company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_CompanyId",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SubCategory",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "SubCategory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ItemType",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "ItemType",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Item",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Item",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "LeadTime",
                table: "Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                filter: "[CompanyId] IS NOT NULL AND [Code] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Company_CompanyId",
                table: "Item",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemType_Company_CompanyId",
                table: "ItemType",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Company_CompanyId",
                table: "SubCategory",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Company_CompanyId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemType_Company_CompanyId",
                table: "ItemType");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Company_CompanyId",
                table: "SubCategory");

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

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ItemType");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "LeadTime",
                table: "Item");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SubCategory",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "ItemType",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Item",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId",
                table: "Category",
                column: "CompanyId");
        }
    }
}
