using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class secondary_sales_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_DamageClaim_DamageClaimId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_VisitPlanners_VisitPlannerId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_VisitStatus_VisitStatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_DamageClaim_DamageClaimId",
                table: "OrderProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_VehicleLoad_VehicleLoadId",
                table: "OrderProcess");

            migrationBuilder.DropTable(
                name: "DamageClaimItems");

            migrationBuilder.DropTable(
                name: "DispatchOrderDetails");

            migrationBuilder.DropTable(
                name: "PaymentBooking");

            migrationBuilder.DropTable(
                name: "VehicleLoadItems");

            migrationBuilder.DropTable(
                name: "VehicleLoadOrders");

            migrationBuilder.DropTable(
                name: "VisitPlanners");

            migrationBuilder.DropTable(
                name: "DamageClaim");

            migrationBuilder.DropTable(
                name: "VehicleLoad");

            migrationBuilder.DropTable(
                name: "VisitStatus");

            migrationBuilder.DropIndex(
                name: "IX_OrderProcess_DamageClaimId",
                table: "OrderProcess");

            migrationBuilder.DropIndex(
                name: "IX_OrderProcess_VehicleLoadId",
                table: "OrderProcess");

            migrationBuilder.DropIndex(
                name: "IX_Order_VisitStatusId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_DamageClaimId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_VisitPlannerId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DamageClaimId",
                table: "OrderProcess");

            migrationBuilder.DropColumn(
                name: "VehicleLoadId",
                table: "OrderProcess");

            migrationBuilder.DropColumn(
                name: "VisitStatusId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DamageClaimId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "VisitPlannerId",
                table: "Attachments");

            migrationBuilder.CreateTable(
                name: "RetailOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<long>(type: "bigint", nullable: true),
                    BillingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Cash = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OnlineTransfer = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransferMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPartial = table.Column<bool>(type: "bit", nullable: true),
                    RetailOrderStatusId = table.Column<long>(type: "bigint", nullable: false),
                    DSFId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_RetailOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetailOrder_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrder_AspNetUsers_DSFId",
                        column: x => x.DSFId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrder_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrder_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrder_Status_RetailOrderStatusId",
                        column: x => x.RetailOrderStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopId = table.Column<long>(type: "bigint", nullable: true),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ShopOrderStatusId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ShopOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrder_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrder_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrder_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrder_Status_ShopOrderStatusId",
                        column: x => x.ShopOrderStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetailOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetailOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ShippedQuantity = table.Column<int>(type: "int", nullable: true),
                    DistributorPromo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistributorPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomDistributorPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TradePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomTradePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetailPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_RetailOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetailOrderItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderItems_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderItems_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RetailOrderItems_RetailOrder_RetailOrderId",
                        column: x => x.RetailOrderId,
                        principalTable: "RetailOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RetailOrderProcess",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RetailOrderId = table.Column<long>(type: "bigint", nullable: true),
                    FromStatusId = table.Column<long>(type: "bigint", nullable: true),
                    ToStatusId = table.Column<long>(type: "bigint", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReject = table.Column<bool>(type: "bit", nullable: true),
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
                    table.PrimaryKey("PK_RetailOrderProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetailOrderProcess_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderProcess_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderProcess_RetailOrder_RetailOrderId",
                        column: x => x.RetailOrderId,
                        principalTable: "RetailOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderProcess_Status_FromStatusId",
                        column: x => x.FromStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RetailOrderProcess_Status_ToStatusId",
                        column: x => x.ToStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopDispatch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    ShopOrderId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ShopDispatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopDispatch_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopDispatch_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopDispatch_ShopOrder_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopDispatch_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopOrderId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ShopOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopOrderItems_ShopOrder_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopDispatchDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShopDispatchId = table.Column<long>(type: "bigint", nullable: false),
                    ShopOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    ShopOrderId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_ShopDispatchDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopDispatchDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopDispatchDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopDispatchDetail_ShopDispatch_ShopDispatchId",
                        column: x => x.ShopDispatchId,
                        principalTable: "ShopDispatch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopDispatchDetail_ShopOrder_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopDispatchDetail_ShopOrderItems_ShopOrderItemId",
                        column: x => x.ShopOrderItemId,
                        principalTable: "ShopOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_CreatedById",
                table: "RetailOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_DSFId",
                table: "RetailOrder",
                column: "DSFId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_ModifiedById",
                table: "RetailOrder",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_RetailOrderStatusId",
                table: "RetailOrder",
                column: "RetailOrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrder_ShopId",
                table: "RetailOrder",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderItems_CreatedById",
                table: "RetailOrderItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderItems_ItemId",
                table: "RetailOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderItems_ModifiedById",
                table: "RetailOrderItems",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderItems_RetailOrderId",
                table: "RetailOrderItems",
                column: "RetailOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderProcess_CreatedById",
                table: "RetailOrderProcess",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderProcess_FromStatusId",
                table: "RetailOrderProcess",
                column: "FromStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderProcess_ModifiedById",
                table: "RetailOrderProcess",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderProcess_RetailOrderId",
                table: "RetailOrderProcess",
                column: "RetailOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RetailOrderProcess_ToStatusId",
                table: "RetailOrderProcess",
                column: "ToStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatch_CreatedById",
                table: "ShopDispatch",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatch_ModifiedById",
                table: "ShopDispatch",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatch_ShopOrderId",
                table: "ShopDispatch",
                column: "ShopOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatch_StatusId",
                table: "ShopDispatch",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatchDetail_CreatedById",
                table: "ShopDispatchDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatchDetail_ModifiedById",
                table: "ShopDispatchDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatchDetail_ShopDispatchId",
                table: "ShopDispatchDetail",
                column: "ShopDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatchDetail_ShopOrderId",
                table: "ShopDispatchDetail",
                column: "ShopOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDispatchDetail_ShopOrderItemId",
                table: "ShopDispatchDetail",
                column: "ShopOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrder_CreatedById",
                table: "ShopOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrder_ModifiedById",
                table: "ShopOrder",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrder_ShopId",
                table: "ShopOrder",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrder_ShopOrderStatusId",
                table: "ShopOrder",
                column: "ShopOrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_CreatedById",
                table: "ShopOrderItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_ItemId",
                table: "ShopOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_ModifiedById",
                table: "ShopOrderItems",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderItems_ShopOrderId",
                table: "ShopOrderItems",
                column: "ShopOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetailOrderItems");

            migrationBuilder.DropTable(
                name: "RetailOrderProcess");

            migrationBuilder.DropTable(
                name: "ShopDispatchDetail");

            migrationBuilder.DropTable(
                name: "RetailOrder");

            migrationBuilder.DropTable(
                name: "ShopDispatch");

            migrationBuilder.DropTable(
                name: "ShopOrderItems");

            migrationBuilder.DropTable(
                name: "ShopOrder");

            migrationBuilder.AddColumn<long>(
                name: "DamageClaimId",
                table: "OrderProcess",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VehicleLoadId",
                table: "OrderProcess",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VisitStatusId",
                table: "Order",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DamageClaimId",
                table: "Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VisitPlannerId",
                table: "Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DamageClaim",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DamageClaimStatusId = table.Column<long>(type: "bigint", nullable: false),
                    DealershipId = table.Column<long>(type: "bigint", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageClaim_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageClaim_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageClaim_Dealerships_DealershipId",
                        column: x => x.DealershipId,
                        principalTable: "Dealerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamageClaim_Status_DamageClaimStatusId",
                        column: x => x.DamageClaimStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentBooking",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(type: "bigint", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    ShopId = table.Column<long>(type: "bigint", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentBooking_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentBooking_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentBooking_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentBooking_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentBooking_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoad",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RouteId = table.Column<long>(type: "bigint", nullable: false),
                    SalesPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    VehicleLoadStatusId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleOutTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_AspNetUsers_SalesPersonId",
                        column: x => x.SalesPersonId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_Status_VehicleLoadStatusId",
                        column: x => x.VehicleLoadStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLoad_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitStatus_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DamageClaimItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimedQuantity = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DamageClaimId = table.Column<long>(type: "bigint", nullable: false),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageClaimItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamageClaimItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageClaimItems_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DamageClaimItems_DamageClaim_DamageClaimId",
                        column: x => x.DamageClaimId,
                        principalTable: "DamageClaim",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamageClaimItems_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispatchOrderDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BiltyNo = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DamageClaimId = table.Column<long>(type: "bigint", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryChallanCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FreightCharges = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    VehicleId = table.Column<long>(type: "bigint", nullable: true),
                    VehicleLoadId = table.Column<long>(type: "bigint", nullable: true),
                    VehicleNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_DamageClaim_DamageClaimId",
                        column: x => x.DamageClaimId,
                        principalTable: "DamageClaim",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DispatchOrderDetails_VehicleLoad_VehicleLoadId",
                        column: x => x.VehicleLoadId,
                        principalTable: "VehicleLoad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoadItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ReturnQuantity = table.Column<int>(type: "int", nullable: true),
                    SoldQuantity = table.Column<int>(type: "int", nullable: true),
                    VehicleLoadId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoadItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleLoadItems_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoadItems_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoadItems_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLoadItems_VehicleLoad_VehicleLoadId",
                        column: x => x.VehicleLoadId,
                        principalTable: "VehicleLoad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoadOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    VehicleLoadId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoadOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleLoadOrders_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoadOrders_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleLoadOrders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLoadOrders_VehicleLoad_VehicleLoadId",
                        column: x => x.VehicleLoadId,
                        principalTable: "VehicleLoad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitPlanners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DSFId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsVisit = table.Column<bool>(type: "bit", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RouteId = table.Column<long>(type: "bigint", nullable: false),
                    ShopId = table.Column<long>(type: "bigint", nullable: false),
                    TerritoryId = table.Column<long>(type: "bigint", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitStatusId = table.Column<long>(type: "bigint", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitPlanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_AspNetUsers_DSFId",
                        column: x => x.DSFId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_Territories_TerritoryId",
                        column: x => x.TerritoryId,
                        principalTable: "Territories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_VisitStatus_VisitStatusId",
                        column: x => x.VisitStatusId,
                        principalTable: "VisitStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitPlanners_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProcess_DamageClaimId",
                table: "OrderProcess",
                column: "DamageClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProcess_VehicleLoadId",
                table: "OrderProcess",
                column: "VehicleLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_VisitStatusId",
                table: "Order",
                column: "VisitStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_DamageClaimId",
                table: "Attachments",
                column: "DamageClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_VisitPlannerId",
                table: "Attachments",
                column: "VisitPlannerId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaim_CreatedById",
                table: "DamageClaim",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaim_DamageClaimStatusId",
                table: "DamageClaim",
                column: "DamageClaimStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaim_DealershipId",
                table: "DamageClaim",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaim_ModifiedById",
                table: "DamageClaim",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaimItems_CreatedById",
                table: "DamageClaimItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaimItems_DamageClaimId",
                table: "DamageClaimItems",
                column: "DamageClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaimItems_ItemId",
                table: "DamageClaimItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageClaimItems_ModifiedById",
                table: "DamageClaimItems",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_CreatedById",
                table: "DispatchOrderDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_DamageClaimId",
                table: "DispatchOrderDetails",
                column: "DamageClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_ModifiedById",
                table: "DispatchOrderDetails",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_OrderId",
                table: "DispatchOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_VehicleId",
                table: "DispatchOrderDetails",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrderDetails_VehicleLoadId",
                table: "DispatchOrderDetails",
                column: "VehicleLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBooking_AccountId",
                table: "PaymentBooking",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBooking_CreatedById",
                table: "PaymentBooking",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBooking_ModifiedById",
                table: "PaymentBooking",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBooking_OrderId",
                table: "PaymentBooking",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBooking_ShopId",
                table: "PaymentBooking",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_CreatedById",
                table: "VehicleLoad",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_ModifiedById",
                table: "VehicleLoad",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_RouteId",
                table: "VehicleLoad",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_SalesPersonId",
                table: "VehicleLoad",
                column: "SalesPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_VehicleId",
                table: "VehicleLoad",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoad_VehicleLoadStatusId",
                table: "VehicleLoad",
                column: "VehicleLoadStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadItems_CreatedById",
                table: "VehicleLoadItems",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadItems_ItemId",
                table: "VehicleLoadItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadItems_ModifiedById",
                table: "VehicleLoadItems",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadItems_VehicleLoadId",
                table: "VehicleLoadItems",
                column: "VehicleLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadOrders_CreatedById",
                table: "VehicleLoadOrders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadOrders_ModifiedById",
                table: "VehicleLoadOrders",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadOrders_OrderId",
                table: "VehicleLoadOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLoadOrders_VehicleLoadId",
                table: "VehicleLoadOrders",
                column: "VehicleLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_CreatedById",
                table: "VisitPlanners",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_DSFId",
                table: "VisitPlanners",
                column: "DSFId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_ModifiedById",
                table: "VisitPlanners",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_RouteId",
                table: "VisitPlanners",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_ShopId",
                table: "VisitPlanners",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_TerritoryId",
                table: "VisitPlanners",
                column: "TerritoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_VisitStatusId",
                table: "VisitPlanners",
                column: "VisitStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlanners_ZoneId",
                table: "VisitPlanners",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitStatus_CreatedById",
                table: "VisitStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitStatus_ModifiedById",
                table: "VisitStatus",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_DamageClaim_DamageClaimId",
                table: "Attachments",
                column: "DamageClaimId",
                principalTable: "DamageClaim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_VisitPlanners_VisitPlannerId",
                table: "Attachments",
                column: "VisitPlannerId",
                principalTable: "VisitPlanners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_VisitStatus_VisitStatusId",
                table: "Order",
                column: "VisitStatusId",
                principalTable: "VisitStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_DamageClaim_DamageClaimId",
                table: "OrderProcess",
                column: "DamageClaimId",
                principalTable: "DamageClaim",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_VehicleLoad_VehicleLoadId",
                table: "OrderProcess",
                column: "VehicleLoadId",
                principalTable: "VehicleLoad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
