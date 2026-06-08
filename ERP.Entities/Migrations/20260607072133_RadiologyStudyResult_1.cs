using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class RadiologyStudyResult_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrder_Status_StatusId",
                table: "RadiologyOrder");

            migrationBuilder.CreateTable(
                name: "RadiologyStudyResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyOrderId = table.Column<long>(type: "bigint", nullable: false),
                    PerformedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClinicalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Impression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conclusion = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RadiologyStudyResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyResult_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyResult_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyResult_AspNetUsers_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyResult_AspNetUsers_ReportedById",
                        column: x => x.ReportedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyResult_RadiologyOrder_RadiologyOrderId",
                        column: x => x.RadiologyOrderId,
                        principalTable: "RadiologyOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyStudyImage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadiologyStudyResultId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SequenceNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RadiologyStudyImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyImage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyImage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyStudyImage_RadiologyStudyResult_RadiologyStudyResultId",
                        column: x => x.RadiologyStudyResultId,
                        principalTable: "RadiologyStudyResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyImage_CreatedById",
                table: "RadiologyStudyImage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyImage_ModifiedById",
                table: "RadiologyStudyImage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyImage_RadiologyStudyResultId",
                table: "RadiologyStudyImage",
                column: "RadiologyStudyResultId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyResult_CreatedById",
                table: "RadiologyStudyResult",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyResult_ModifiedById",
                table: "RadiologyStudyResult",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyResult_PerformedById",
                table: "RadiologyStudyResult",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyResult_RadiologyOrderId",
                table: "RadiologyStudyResult",
                column: "RadiologyOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyStudyResult_ReportedById",
                table: "RadiologyStudyResult",
                column: "ReportedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrder_AppointmentStatus_StatusId",
                table: "RadiologyOrder",
                column: "StatusId",
                principalTable: "AppointmentStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrder_AppointmentStatus_StatusId",
                table: "RadiologyOrder");

            migrationBuilder.DropTable(
                name: "RadiologyStudyImage");

            migrationBuilder.DropTable(
                name: "RadiologyStudyResult");

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrder_Status_StatusId",
                table: "RadiologyOrder",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
