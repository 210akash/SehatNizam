using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ServiceAccount_PaymentMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointmentPaymentId",
                table: "TransactionDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaymentModeId",
                table: "ServiceAccount",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetail_AppointmentPaymentId",
                table: "TransactionDetail",
                column: "AppointmentPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_PaymentModeId",
                table: "ServiceAccount",
                column: "PaymentModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAccount_PaymentMode_PaymentModeId",
                table: "ServiceAccount",
                column: "PaymentModeId",
                principalTable: "PaymentMode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetail_AppointmentPayment_AppointmentPaymentId",
                table: "TransactionDetail",
                column: "AppointmentPaymentId",
                principalTable: "AppointmentPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAccount_PaymentMode_PaymentModeId",
                table: "ServiceAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetail_AppointmentPayment_AppointmentPaymentId",
                table: "TransactionDetail");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetail_AppointmentPaymentId",
                table: "TransactionDetail");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAccount_PaymentModeId",
                table: "ServiceAccount");

            migrationBuilder.DropColumn(
                name: "AppointmentPaymentId",
                table: "TransactionDetail");

            migrationBuilder.DropColumn(
                name: "PaymentModeId",
                table: "ServiceAccount");
        }
    }
}
