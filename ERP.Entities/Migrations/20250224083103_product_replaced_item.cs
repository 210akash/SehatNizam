using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class product_replaced_item : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Product_ProductId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageClaimItems_Product_ProductId",
                table: "DamageClaimItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Product_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceGroupDetails_Product_ProductId",
                table: "PriceGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLoadItems_Product_ProductId",
                table: "VehicleLoadItems");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ProductId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Attachments");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "VehicleLoadItems",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleLoadItems_ProductId",
                table: "VehicleLoadItems",
                newName: "IX_VehicleLoadItems_ItemId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "PriceGroupDetails",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_PriceGroupDetails_ProductId",
                table: "PriceGroupDetails",
                newName: "IX_PriceGroupDetails_ItemId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItems",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ItemId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "DamageClaimItems",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_DamageClaimItems_ProductId",
                table: "DamageClaimItems",
                newName: "IX_DamageClaimItems_ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageClaimItems_Item_ItemId",
                table: "DamageClaimItems",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Item_ItemId",
                table: "OrderItems",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceGroupDetails_Item_ItemId",
                table: "PriceGroupDetails",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLoadItems_Item_ItemId",
                table: "VehicleLoadItems",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageClaimItems_Item_ItemId",
                table: "DamageClaimItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Item_ItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceGroupDetails_Item_ItemId",
                table: "PriceGroupDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLoadItems_Item_ItemId",
                table: "VehicleLoadItems");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "VehicleLoadItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleLoadItems_ItemId",
                table: "VehicleLoadItems",
                newName: "IX_VehicleLoadItems_ProductId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "PriceGroupDetails",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_PriceGroupDetails_ItemId",
                table: "PriceGroupDetails",
                newName: "IX_PriceGroupDetails_ProductId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "OrderItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ItemId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "DamageClaimItems",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DamageClaimItems_ItemId",
                table: "DamageClaimItems",
                newName: "IX_DamageClaimItems_ProductId");

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistributorPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpoRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantityInPack = table.Column<int>(type: "int", nullable: false),
                    RetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TradePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VolumeInMl = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ProductId",
                table: "Attachments",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CreatedById",
                table: "Product",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ModifiedById",
                table: "Product",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Product_ProductId",
                table: "Attachments",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DamageClaimItems_Product_ProductId",
                table: "DamageClaimItems",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Product_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceGroupDetails_Product_ProductId",
                table: "PriceGroupDetails",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLoadItems_Product_ProductId",
                table: "VehicleLoadItems",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
