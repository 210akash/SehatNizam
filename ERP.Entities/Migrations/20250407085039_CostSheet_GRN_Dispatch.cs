using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class CostSheet_GRN_Dispatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CostSheetId",
                table: "GRNDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CostSheetId",
                table: "DispatchDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRNDetail_CostSheetId",
                table: "GRNDetail",
                column: "CostSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchDetail_CostSheetId",
                table: "DispatchDetail",
                column: "CostSheetId");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchDetail_CostSheet_CostSheetId",
                table: "DispatchDetail",
                column: "CostSheetId",
                principalTable: "CostSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GRNDetail_CostSheet_CostSheetId",
                table: "GRNDetail",
                column: "CostSheetId",
                principalTable: "CostSheet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchDetail_CostSheet_CostSheetId",
                table: "DispatchDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_GRNDetail_CostSheet_CostSheetId",
                table: "GRNDetail");

            migrationBuilder.DropIndex(
                name: "IX_GRNDetail_CostSheetId",
                table: "GRNDetail");

            migrationBuilder.DropIndex(
                name: "IX_DispatchDetail_CostSheetId",
                table: "DispatchDetail");

            migrationBuilder.DropColumn(
                name: "CostSheetId",
                table: "GRNDetail");

            migrationBuilder.DropColumn(
                name: "CostSheetId",
                table: "DispatchDetail");
        }
    }
}
