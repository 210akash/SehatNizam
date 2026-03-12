using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Project_ISS_SM_Dispach_PR_SP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleReturnDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleMaterialDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "PurchaseReturnDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "IssuanceDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "DispatchDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturnDetail_ProjectId",
                table: "SaleReturnDetail",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialDetail_ProjectId",
                table: "SaleMaterialDetail",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseReturnDetail_ProjectId",
                table: "PurchaseReturnDetail",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_ProjectId",
                table: "IssuanceDetail",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchDetail_ProjectId",
                table: "DispatchDetail",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchDetail_Project_ProjectId",
                table: "DispatchDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuanceDetail_Project_ProjectId",
                table: "IssuanceDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseReturnDetail_Project_ProjectId",
                table: "PurchaseReturnDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleMaterialDetail_Project_ProjectId",
                table: "SaleMaterialDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturnDetail_Project_ProjectId",
                table: "SaleReturnDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchDetail_Project_ProjectId",
                table: "DispatchDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuanceDetail_Project_ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseReturnDetail_Project_ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleMaterialDetail_Project_ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturnDetail_Project_ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturnDetail_ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_SaleMaterialDetail_ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseReturnDetail_ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_IssuanceDetail_ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.DropIndex(
                name: "IX_DispatchDetail_ProjectId",
                table: "DispatchDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleMaterialDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseReturnDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "IssuanceDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "DispatchDetail");
        }
    }
}
