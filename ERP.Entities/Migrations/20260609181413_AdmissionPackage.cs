using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class AdmissionPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionPackageMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionPackageMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionPackageMaster_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionPackageMaster_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionPackageDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionPackageMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionPackageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionPackageDetail_AdmissionPackageMaster_AdmissionPackageMasterId",
                        column: x => x.AdmissionPackageMasterId,
                        principalTable: "AdmissionPackageMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdmissionPackageDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionPackageDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionPackageDetail_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageDetail_AdmissionPackageMasterId",
                table: "AdmissionPackageDetail",
                column: "AdmissionPackageMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageDetail_CreatedById",
                table: "AdmissionPackageDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageDetail_ModifiedById",
                table: "AdmissionPackageDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageDetail_ServiceId",
                table: "AdmissionPackageDetail",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageMaster_CreatedById",
                table: "AdmissionPackageMaster",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionPackageMaster_ModifiedById",
                table: "AdmissionPackageMaster",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionPackageDetail");

            migrationBuilder.DropTable(
                name: "AdmissionPackageMaster");
        }
    }
}
