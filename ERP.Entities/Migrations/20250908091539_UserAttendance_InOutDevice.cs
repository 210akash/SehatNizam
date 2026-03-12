using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class UserAttendance_InOutDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryStore",
                table: "CategoryStore");

            migrationBuilder.AddColumn<long>(
                name: "InDeviceId",
                table: "UserAttendance",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OutDeviceId",
                table: "UserAttendance",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryStore",
                table: "CategoryStore",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttendance_InDeviceId",
                table: "UserAttendance",
                column: "InDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttendance_OutDeviceId",
                table: "UserAttendance",
                column: "OutDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryStore_CategoryId",
                table: "CategoryStore",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_Device_InDeviceId",
                table: "UserAttendance",
                column: "InDeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAttendance_Device_OutDeviceId",
                table: "UserAttendance",
                column: "OutDeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_Device_InDeviceId",
                table: "UserAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAttendance_Device_OutDeviceId",
                table: "UserAttendance");

            migrationBuilder.DropIndex(
                name: "IX_UserAttendance_InDeviceId",
                table: "UserAttendance");

            migrationBuilder.DropIndex(
                name: "IX_UserAttendance_OutDeviceId",
                table: "UserAttendance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryStore",
                table: "CategoryStore");

            migrationBuilder.DropIndex(
                name: "IX_CategoryStore_CategoryId",
                table: "CategoryStore");

            migrationBuilder.DropColumn(
                name: "InDeviceId",
                table: "UserAttendance");

            migrationBuilder.DropColumn(
                name: "OutDeviceId",
                table: "UserAttendance");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryStore",
                table: "CategoryStore",
                columns: new[] { "CategoryId", "StoreId" });
        }
    }
}
