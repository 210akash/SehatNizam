using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class INVCODE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CancelDispatchId",
                table: "OrderProcess",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "INVCode",
                table: "DispatchOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CancelDispatch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_CancelDispatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancelDispatch_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelDispatch_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelDispatch_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CancelDispatch_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancelDispatchDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CancelDispatchId = table.Column<long>(type: "bigint", nullable: false),
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CancelDispatchDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancelDispatchDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelDispatchDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CancelDispatchDetail_CancelDispatch_CancelDispatchId",
                        column: x => x.CancelDispatchId,
                        principalTable: "CancelDispatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CancelDispatchDetail_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProcess_CancelDispatchId",
                table: "OrderProcess",
                column: "CancelDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatch_CreatedById",
                table: "CancelDispatch",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatch_ModifiedById",
                table: "CancelDispatch",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatch_OrderId",
                table: "CancelDispatch",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatch_StatusId",
                table: "CancelDispatch",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatchDetail_CancelDispatchId",
                table: "CancelDispatchDetail",
                column: "CancelDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatchDetail_CreatedById",
                table: "CancelDispatchDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatchDetail_ModifiedById",
                table: "CancelDispatchDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CancelDispatchDetail_OrderItemId",
                table: "CancelDispatchDetail",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_CancelDispatch_CancelDispatchId",
                table: "OrderProcess",
                column: "CancelDispatchId",
                principalTable: "CancelDispatch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_CancelDispatch_CancelDispatchId",
                table: "OrderProcess");

            migrationBuilder.DropTable(
                name: "CancelDispatchDetail");

            migrationBuilder.DropTable(
                name: "CancelDispatch");

            migrationBuilder.DropIndex(
                name: "IX_OrderProcess_CancelDispatchId",
                table: "OrderProcess");

            migrationBuilder.DropColumn(
                name: "CancelDispatchId",
                table: "OrderProcess");

            migrationBuilder.DropColumn(
                name: "INVCode",
                table: "DispatchOrder");
        }
    }
}
