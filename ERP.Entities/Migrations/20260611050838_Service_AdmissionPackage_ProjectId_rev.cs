using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Service_AdmissionPackage_ProjectId_rev : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdmissionPackageMaster_Project_ProjectId",
                table: "AdmissionPackageMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_Project_ProjectId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_ProjectId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_AdmissionPackageMaster_ProjectId",
                table: "AdmissionPackageMaster");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AdmissionPackageMaster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "Service",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "AdmissionPackageMaster",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Service_ProjectId",
                table: "Service",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageMaster_ProjectId",
                table: "AdmissionPackageMaster",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdmissionPackageMaster_Project_ProjectId",
                table: "AdmissionPackageMaster",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Project_ProjectId",
                table: "Service",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
