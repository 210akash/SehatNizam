using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class item_orderstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageClaim_OrderStatus_DamageClaimStatusId",
                table: "DamageClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_DSFId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_OrderStatus_FromStatusId",
                table: "OrderProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_OrderStatus_ToStatusId",
                table: "OrderProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLoad_OrderStatus_VehicleLoadStatusId",
                table: "VehicleLoad");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Status",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Status",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "Status",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Status",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Status",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Status",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Status",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Volume",
                table: "Item",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Status_CreatedById",
                table: "Status",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Status_ModifiedById",
                table: "Status",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageClaim_Status_DamageClaimStatusId",
                table: "DamageClaim",
                column: "DamageClaimStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_DSFId",
                table: "Order",
                column: "DSFId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Status_OrderStatusId",
                table: "Order",
                column: "OrderStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_Status_FromStatusId",
                table: "OrderProcess",
                column: "FromStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_Status_ToStatusId",
                table: "OrderProcess",
                column: "ToStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Status_AspNetUsers_CreatedById",
                table: "Status",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Status_AspNetUsers_ModifiedById",
                table: "Status",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLoad_Status_VehicleLoadStatusId",
                table: "VehicleLoad",
                column: "VehicleLoadStatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageClaim_Status_DamageClaimStatusId",
                table: "DamageClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_DSFId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Status_OrderStatusId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_Status_FromStatusId",
                table: "OrderProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcess_Status_ToStatusId",
                table: "OrderProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_Status_AspNetUsers_CreatedById",
                table: "Status");

            migrationBuilder.DropForeignKey(
                name: "FK_Status_AspNetUsers_ModifiedById",
                table: "Status");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleLoad_Status_VehicleLoadStatusId",
                table: "VehicleLoad");

            migrationBuilder.DropIndex(
                name: "IX_Status_CreatedById",
                table: "Status");

            migrationBuilder.DropIndex(
                name: "IX_Status_ModifiedById",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Status");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Item");

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatus_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_CreatedById",
                table: "OrderStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_ModifiedById",
                table: "OrderStatus",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageClaim_OrderStatus_DamageClaimStatusId",
                table: "DamageClaim",
                column: "DamageClaimStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_DSFId",
                table: "Order",
                column: "DSFId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusId",
                table: "Order",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_OrderStatus_FromStatusId",
                table: "OrderProcess",
                column: "FromStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcess_OrderStatus_ToStatusId",
                table: "OrderProcess",
                column: "ToStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleLoad_OrderStatus_VehicleLoadStatusId",
                table: "VehicleLoad",
                column: "VehicleLoadStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
