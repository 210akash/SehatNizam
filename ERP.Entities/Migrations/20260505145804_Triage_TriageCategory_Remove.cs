using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Triage_TriageCategory_Remove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triage_TriageCategory_TriageCategoryId",
                table: "Triage");

            migrationBuilder.DropTable(
                name: "TriageCategory");

            migrationBuilder.DropIndex(
                name: "IX_Triage_TriageCategoryId",
                table: "Triage");

            migrationBuilder.DropColumn(
                name: "TriageCategoryId",
                table: "Triage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TriageCategoryId",
                table: "Triage",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TriageCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_TriageCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriageCategory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriageCategory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Triage_TriageCategoryId",
                table: "Triage",
                column: "TriageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TriageCategory_CreatedById",
                table: "TriageCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TriageCategory_ModifiedById",
                table: "TriageCategory",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Triage_TriageCategory_TriageCategoryId",
                table: "Triage",
                column: "TriageCategoryId",
                principalTable: "TriageCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
