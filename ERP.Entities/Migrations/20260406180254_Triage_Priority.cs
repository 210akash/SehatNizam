using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Triage_Priority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triage_TriagePriority_TriagePriorityId",
                table: "Triage");

            migrationBuilder.DropTable(
                name: "TriagePriority");

            migrationBuilder.AddForeignKey(
                name: "FK_Triage_PriorityLevel_TriagePriorityId",
                table: "Triage",
                column: "TriagePriorityId",
                principalTable: "PriorityLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triage_PriorityLevel_TriagePriorityId",
                table: "Triage");

            migrationBuilder.CreateTable(
                name: "TriagePriority",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriagePriority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriagePriority_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriagePriority_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriagePriority_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_CompanyId",
                table: "TriagePriority",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_CreatedById",
                table: "TriagePriority",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_ModifiedById",
                table: "TriagePriority",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Triage_TriagePriority_TriagePriorityId",
                table: "Triage",
                column: "TriagePriorityId",
                principalTable: "TriagePriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
