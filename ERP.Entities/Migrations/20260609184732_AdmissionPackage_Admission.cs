using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AdmissionPackage_Admission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdmissionPackageMasterId",
                table: "Admission",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Admission_AdmissionPackageMasterId",
                table: "Admission",
                column: "AdmissionPackageMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admission_AdmissionPackageMaster_AdmissionPackageMasterId",
                table: "Admission",
                column: "AdmissionPackageMasterId",
                principalTable: "AdmissionPackageMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admission_AdmissionPackageMaster_AdmissionPackageMasterId",
                table: "Admission");

            migrationBuilder.DropIndex(
                name: "IX_Admission_AdmissionPackageMasterId",
                table: "Admission");

            migrationBuilder.DropColumn(
                name: "AdmissionPackageMasterId",
                table: "Admission");
        }
    }
}
