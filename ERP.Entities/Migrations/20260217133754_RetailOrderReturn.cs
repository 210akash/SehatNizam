using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class RetailOrderReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RetailOrderReturn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    RetailOrderId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RetailOrderReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturn_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturn_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturn_RetailOrder_RetailOrderId",
                        column: x => x.RetailOrderId,
                        principalTable: "RetailOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturn_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RetailOrderReturnDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetailOrderReturnId = table.Column<long>(type: "bigint", nullable: false),
                    RetailOrderItemsId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RetailOrderReturnDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturnDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturnDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturnDetail_RetailOrderItems_RetailOrderItemsId",
                        column: x => x.RetailOrderItemsId,
                        principalTable: "RetailOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderReturnDetail_RetailOrderReturn_RetailOrderReturnId",
                        column: x => x.RetailOrderReturnId,
                        principalTable: "RetailOrderReturn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturn_CreatedById",
                table: "RetailOrderReturn",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturn_ModifiedById",
                table: "RetailOrderReturn",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturn_RetailOrderId",
                table: "RetailOrderReturn",
                column: "RetailOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturn_StatusId",
                table: "RetailOrderReturn",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturnDetail_CreatedById",
                table: "RetailOrderReturnDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturnDetail_ModifiedById",
                table: "RetailOrderReturnDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturnDetail_RetailOrderItemsId",
                table: "RetailOrderReturnDetail",
                column: "RetailOrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderReturnDetail_RetailOrderReturnId",
                table: "RetailOrderReturnDetail",
                column: "RetailOrderReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetailOrderReturnDetail");

            migrationBuilder.DropTable(
                name: "RetailOrderReturn");
        }
    }
}
