using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class SaleReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleReturn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DispatchOrderId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_SaleReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleReturn_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleReturn_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleReturn_DispatchOrder_DispatchOrderId",
                        column: x => x.DispatchOrderId,
                        principalTable: "DispatchOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleReturn_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaleReturnDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleReturnId = table.Column<long>(type: "bigint", nullable: false),
                    DispatchDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_SaleReturnDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetail_DispatchDetail_DispatchDetailId",
                        column: x => x.DispatchDetailId,
                        principalTable: "DispatchDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleReturnDetail_SaleReturn_SaleReturnId",
                        column: x => x.SaleReturnId,
                        principalTable: "SaleReturn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_CreatedById",
                table: "SaleReturn",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_DispatchOrderId",
                table: "SaleReturn",
                column: "DispatchOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_ModifiedById",
                table: "SaleReturn",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_StatusId",
                table: "SaleReturn",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetail_CreatedById",
                table: "SaleReturnDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetail_DispatchDetailId",
                table: "SaleReturnDetail",
                column: "DispatchDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetail_ModifiedById",
                table: "SaleReturnDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetail_SaleReturnId",
                table: "SaleReturnDetail",
                column: "SaleReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleReturnDetail");

            migrationBuilder.DropTable(
                name: "SaleReturn");
        }
    }
}
