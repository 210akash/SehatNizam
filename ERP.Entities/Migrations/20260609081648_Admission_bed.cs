using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Admission_bed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admission_Bed_BedId",
                table: "Admission");

            migrationBuilder.DropIndex(
                name: "IX_Admission_BedId",
                table: "Admission");

            migrationBuilder.DropColumn(
                name: "BedId",
                table: "Admission");

            migrationBuilder.CreateTable(
                name: "AdmissionBed",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    BedId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_AdmissionBed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionBed_Admission_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdmissionBed_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionBed_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionBed_Bed_BedId",
                        column: x => x.BedId,
                        principalTable: "Bed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionBed_AdmissionId",
                table: "AdmissionBed",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionBed_BedId",
                table: "AdmissionBed",
                column: "BedId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionBed_CreatedById",
                table: "AdmissionBed",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionBed_ModifiedById",
                table: "AdmissionBed",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionBed");

            migrationBuilder.AddColumn<long>(
                name: "BedId",
                table: "Admission",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admission_BedId",
                table: "Admission",
                column: "BedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admission_Bed_BedId",
                table: "Admission",
                column: "BedId",
                principalTable: "Bed",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
