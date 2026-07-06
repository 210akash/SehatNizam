using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AppointmentPayment_approvalhistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<long>(
            //    name: "VoucherTypeId",
            //    table: "PaymentMode",
            //    type: "bigint",
            //    nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedById",
                table: "AppointmentPayment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "AppointmentPayment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "AppointmentPayment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "AppointmentPayment",
                type: "datetime2",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_PaymentMode_VoucherTypeId",
            //    table: "PaymentMode",
            //    column: "VoucherTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_ApprovedById",
                table: "AppointmentPayment",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_ProcessedById",
                table: "AppointmentPayment",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentPayment_AspNetUsers_ApprovedById",
                table: "AppointmentPayment",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentPayment_AspNetUsers_ProcessedById",
                table: "AppointmentPayment",
                column: "ProcessedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_PaymentMode_VoucherType_VoucherTypeId",
            //    table: "PaymentMode",
            //    column: "VoucherTypeId",
            //    principalTable: "VoucherType",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentPayment_AspNetUsers_ApprovedById",
                table: "AppointmentPayment");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentPayment_AspNetUsers_ProcessedById",
                table: "AppointmentPayment");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_PaymentMode_VoucherType_VoucherTypeId",
            //    table: "PaymentMode");

            //migrationBuilder.DropIndex(
            //    name: "IX_PaymentMode_VoucherTypeId",
            //    table: "PaymentMode");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentPayment_ApprovedById",
                table: "AppointmentPayment");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentPayment_ProcessedById",
                table: "AppointmentPayment");

            //migrationBuilder.DropColumn(
            //    name: "VoucherTypeId",
            //    table: "PaymentMode");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "AppointmentPayment");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "AppointmentPayment");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "AppointmentPayment");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "AppointmentPayment");
        }
    }
}
