using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class IssuanceDetail_CostSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CostSheetId",
                table: "IssuanceDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_CostSheetId",
                table: "IssuanceDetail",
                column: "CostSheetId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssuanceDetail_CostSheet_CostSheetId",
                table: "IssuanceDetail",
                column: "CostSheetId",
                principalTable: "CostSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssuanceDetail_CostSheet_CostSheetId",
                table: "IssuanceDetail");

            migrationBuilder.DropIndex(
                name: "IX_IssuanceDetail_CostSheetId",
                table: "IssuanceDetail");

            migrationBuilder.DropColumn(
                name: "CostSheetId",
                table: "IssuanceDetail");
        }
    }
}
