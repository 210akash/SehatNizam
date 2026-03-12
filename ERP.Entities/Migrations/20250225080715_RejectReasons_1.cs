using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class RejectReasons_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail");

            migrationBuilder.AlterColumn<long>(
                name: "RejectReasonId",
                table: "InspectionDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail",
                column: "RejectReasonId",
                principalTable: "RejectReason",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail");

            migrationBuilder.AlterColumn<long>(
                name: "RejectReasonId",
                table: "InspectionDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail",
                column: "RejectReasonId",
                principalTable: "RejectReason",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
