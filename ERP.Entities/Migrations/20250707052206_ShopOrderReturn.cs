using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ShopOrderReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopOrderReturn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturn_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturn_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturn_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturn_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderReturnDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderReturnId = table.Column<long>(type: "bigint", nullable: false),
                    OrderItemsId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderReturnDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturnDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturnDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturnDetail_OrderItems_OrderItemsId",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderReturnDetail_ShopOrderReturn_ShopOrderReturnId",
                        column: x => x.ShopOrderReturnId,
                        principalTable: "ShopOrderReturn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturn_CreatedById",
                table: "ShopOrderReturn",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturn_ModifiedById",
                table: "ShopOrderReturn",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturn_OrderId",
                table: "ShopOrderReturn",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturn_StatusId",
                table: "ShopOrderReturn",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturnDetail_CreatedById",
                table: "ShopOrderReturnDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturnDetail_ModifiedById",
                table: "ShopOrderReturnDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturnDetail_OrderItemsId",
                table: "ShopOrderReturnDetail",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderReturnDetail_ShopOrderReturnId",
                table: "ShopOrderReturnDetail",
                column: "ShopOrderReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopOrderReturnDetail");

            migrationBuilder.DropTable(
                name: "ShopOrderReturn");
        }
    }
}
