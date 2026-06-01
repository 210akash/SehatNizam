using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class LaborderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_Status_StatusId",
                table: "LabOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_AppointmentStatus_StatusId",
                table: "LabOrder",
                column: "StatusId",
                principalTable: "AppointmentStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_AppointmentStatus_StatusId",
                table: "LabOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_Status_StatusId",
                table: "LabOrder",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
