using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class recieved_change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_AspNetUsers_UserId",
                table: "UserAttendance");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceivedById",
                table: "DispatchOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "DispatchOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "DispatchOrder",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_ReceivedById",
                table: "DispatchOrder",
                column: "ReceivedById");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchOrder_StatusId",
                table: "DispatchOrder",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchOrder_AspNetUsers_ReceivedById",
                table: "DispatchOrder",
                column: "ReceivedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchOrder_Status_StatusId",
                table: "DispatchOrder",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_AspNetUsers_UserId",
                table: "UserAttendance",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatchOrder_AspNetUsers_ReceivedById",
                table: "DispatchOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatchOrder_Status_StatusId",
                table: "DispatchOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_AspNetUsers_UserId",
                table: "UserAttendance");

            migrationBuilder.DropIndex(
                name: "IX_DispatchOrder_ReceivedById",
                table: "DispatchOrder");

            migrationBuilder.DropIndex(
                name: "IX_DispatchOrder_StatusId",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "ReceivedById",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "DispatchOrder");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "DispatchOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_UserId",
                table: "Attachments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_AspNetUsers_UserId",
                table: "UserAttendance",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
