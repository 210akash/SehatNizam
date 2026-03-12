using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ComparativeStatement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ComparativeStatementVendorId",
                table: "PurchaseOrderDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ComparativeStatement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparativeStatement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparativeStatement_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatement_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatement_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatement_AspNetUsers_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatement_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComparativeStatementDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparativeStatementId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ComparativeStatementDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementDetail_ComparativeStatement_ComparativeStatementId",
                        column: x => x.ComparativeStatementId,
                        principalTable: "ComparativeStatement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementDetail_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComparativeStatementVendor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparativeStatementDetailId = table.Column<long>(type: "bigint", nullable: false),
                    VendorId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_ComparativeStatementVendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementVendor_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementVendor_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementVendor_ComparativeStatementDetail_ComparativeStatementDetailId",
                        column: x => x.ComparativeStatementDetailId,
                        principalTable: "ComparativeStatementDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparativeStatementVendor_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetail_ComparativeStatementVendorId",
                table: "PurchaseOrderDetail",
                column: "ComparativeStatementVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_ApprovedById",
                table: "ComparativeStatement",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_CreatedById",
                table: "ComparativeStatement",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_ModifiedById",
                table: "ComparativeStatement",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_ProcessedById",
                table: "ComparativeStatement",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatement_StatusId",
                table: "ComparativeStatement",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_ComparativeStatementId",
                table: "ComparativeStatementDetail",
                column: "ComparativeStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_CreatedById",
                table: "ComparativeStatementDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_ItemId",
                table: "ComparativeStatementDetail",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementDetail_ModifiedById",
                table: "ComparativeStatementDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementVendor_ComparativeStatementDetailId",
                table: "ComparativeStatementVendor",
                column: "ComparativeStatementDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementVendor_CreatedById",
                table: "ComparativeStatementVendor",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementVendor_ModifiedById",
                table: "ComparativeStatementVendor",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ComparativeStatementVendor_VendorId",
                table: "ComparativeStatementVendor",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetail_ComparativeStatementVendor_ComparativeStatementVendorId",
                table: "PurchaseOrderDetail",
                column: "ComparativeStatementVendorId",
                principalTable: "ComparativeStatementVendor",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetail_ComparativeStatementVendor_ComparativeStatementVendorId",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropTable(
                name: "ComparativeStatementVendor");

            migrationBuilder.DropTable(
                name: "ComparativeStatementDetail");

            migrationBuilder.DropTable(
                name: "ComparativeStatement");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderDetail_ComparativeStatementVendorId",
                table: "PurchaseOrderDetail");

            migrationBuilder.DropColumn(
                name: "ComparativeStatementVendorId",
                table: "PurchaseOrderDetail");
        }
    }
}
