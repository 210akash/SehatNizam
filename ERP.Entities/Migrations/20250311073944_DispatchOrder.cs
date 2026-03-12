using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class DispatchOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatch_Order_OrderId",
                table: "Dispatch");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatchDetail_Dispatch_DispatchId",
                table: "DispatchDetail");

            migrationBuilder.DropIndex(
                name: "IX_Dispatch_OrderId",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "DCCode",
                table: "DispatchDetail");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Dispatch");

            migrationBuilder.RenameColumn(
                name: "DispatchId",
                table: "DispatchDetail",
                newName: "DispatchOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_DispatchDetail_DispatchId",
                table: "DispatchDetail",
                newName: "IX_DispatchDetail_DispatchOrderId");

            migrationBuilder.CreateTable(
                name: "DispatchOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    DCCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DispatchId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_DispatchOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchOrder_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrder_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrder_Dispatch_DispatchId",
                        column: x => x.DispatchId,
                        principalTable: "Dispatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DispatchOrder_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_CreatedById",
                table: "DispatchOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_DispatchId",
                table: "DispatchOrder",
                column: "DispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_ModifiedById",
                table: "DispatchOrder",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_OrderId",
                table: "DispatchOrder",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchDetail_DispatchOrder_DispatchOrderId",
                table: "DispatchDetail",
                column: "DispatchOrderId",
                principalTable: "DispatchOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchDetail_DispatchOrder_DispatchOrderId",
                table: "DispatchDetail");

            migrationBuilder.DropTable(
                name: "DispatchOrder");

            migrationBuilder.RenameColumn(
                name: "DispatchOrderId",
                table: "DispatchDetail",
                newName: "DispatchId");

            migrationBuilder.RenameIndex(
                name: "IX_DispatchDetail_DispatchOrderId",
                table: "DispatchDetail",
                newName: "IX_DispatchDetail_DispatchId");

            migrationBuilder.AddColumn<string>(
                name: "DCCode",
                table: "DispatchDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "Dispatch",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Dispatch_OrderId",
                table: "Dispatch",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatch_Order_OrderId",
                table: "Dispatch",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchDetail_Dispatch_DispatchId",
                table: "DispatchDetail",
                column: "DispatchId",
                principalTable: "Dispatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
