using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ServiceAccount_ServiceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAccount_Service_ServiceId",
                table: "ServiceAccount");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ServiceAccount",
                newName: "ServiceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceAccount_ServiceId",
                table: "ServiceAccount",
                newName: "IX_ServiceAccount_ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAccount_ServiceType_ServiceTypeId",
                table: "ServiceAccount",
                column: "ServiceTypeId",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAccount_ServiceType_ServiceTypeId",
                table: "ServiceAccount");

            migrationBuilder.RenameColumn(
                name: "ServiceTypeId",
                table: "ServiceAccount",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceAccount_ServiceTypeId",
                table: "ServiceAccount",
                newName: "IX_ServiceAccount_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAccount_Service_ServiceId",
                table: "ServiceAccount",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
