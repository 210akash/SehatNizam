using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Referrer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "LabOrder");

            migrationBuilder.AddColumn<long>(
                name: "ReferrerId",
                table: "Appointment",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Referrer",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hospital = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_Referrer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Referrer_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrer_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Referrer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ReferrerId",
                table: "Appointment",
                column: "ReferrerId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_CompanyId",
                table: "Referrer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_CreatedById",
                table: "Referrer",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_ModifiedById",
                table: "Referrer",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Referrer_ReferrerId",
                table: "Appointment",
                column: "ReferrerId",
                principalTable: "Referrer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Referrer_ReferrerId",
                table: "Appointment");

            migrationBuilder.DropTable(
                name: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ReferrerId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "ReferrerId",
                table: "Appointment");

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "LabOrder",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
