using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PaymentMode_VchType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VoucherTypeId",
                table: "PaymentMode",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMode_VoucherTypeId",
                table: "PaymentMode",
                column: "VoucherTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMode_VoucherType_VoucherTypeId",
                table: "PaymentMode",
                column: "VoucherTypeId",
                principalTable: "VoucherType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMode_VoucherType_VoucherTypeId",
                table: "PaymentMode");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMode_VoucherTypeId",
                table: "PaymentMode");

            migrationBuilder.DropColumn(
                name: "VoucherTypeId",
                table: "PaymentMode");
        }
    }
}
