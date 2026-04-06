using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Triage_Revm_Patient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triage_AspNetUsers_PatientId",
                table: "Triage");

            migrationBuilder.DropIndex(
                name: "IX_Triage_PatientId",
                table: "Triage");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Triage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "Triage",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Triage_PatientId",
                table: "Triage",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Triage_AspNetUsers_PatientId",
                table: "Triage",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
