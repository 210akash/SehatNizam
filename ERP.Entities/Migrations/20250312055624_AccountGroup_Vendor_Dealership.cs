using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AccountGroup_Vendor_Dealership : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "PurchaseDemand",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "DealershipId",
                table: "AccountGroup",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VendorId",
                table: "AccountGroup",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_DealershipId",
                table: "AccountGroup",
                column: "DealershipId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_VendorId",
                table: "AccountGroup",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountGroup_Dealerships_DealershipId",
                table: "AccountGroup",
                column: "DealershipId",
                principalTable: "Dealerships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountGroup_Vendor_VendorId",
                table: "AccountGroup",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountGroup_Dealerships_DealershipId",
                table: "AccountGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountGroup_Vendor_VendorId",
                table: "AccountGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroup_DealershipId",
                table: "AccountGroup");

            migrationBuilder.DropIndex(
                name: "IX_AccountGroup_VendorId",
                table: "AccountGroup");

            migrationBuilder.DropColumn(
                name: "DealershipId",
                table: "AccountGroup");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "AccountGroup");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestDate",
                table: "PurchaseDemand",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
