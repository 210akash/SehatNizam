using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Admission_23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admission_Ward_WardId",
                table: "Admission");

            migrationBuilder.DropIndex(
                name: "IX_Admission_WardId",
                table: "Admission");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Admission");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPackageAmount",
                table: "Admission",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPackageAmount",
                table: "Admission");

            migrationBuilder.AddColumn<long>(
                name: "WardId",
                table: "Admission",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admission_WardId",
                table: "Admission",
                column: "WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admission_Ward_WardId",
                table: "Admission",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
