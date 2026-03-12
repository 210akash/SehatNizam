using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PurchaseReturn_Project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReturnDetail_Project_ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturnDetail_ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "PurchaseReturn",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturn_ProjectId",
                table: "PurchaseReturn",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseReturn_Project_ProjectId",
                table: "PurchaseReturn",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReturn_Project_ProjectId",
                table: "PurchaseReturn");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturn_ProjectId",
                table: "PurchaseReturn");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseReturn");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "PurchaseReturnDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnDetail_ProjectId",
                table: "PurchaseReturnDetail",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseReturnDetail_Project_ProjectId",
                table: "PurchaseReturnDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
