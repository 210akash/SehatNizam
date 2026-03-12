using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class SaleReturnDispatch_project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchDetail_Project_ProjectId",
                table: "DispatchDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturnDetail_Project_ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturnDetail_ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropIndex(
                name: "IX_DispatchDetail_ProjectId",
                table: "DispatchDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleReturnDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "DispatchDetail");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleReturn",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "Dispatch",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleReturn_ProjectId",
                table: "SaleReturn",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispatch_ProjectId",
                table: "Dispatch",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispatch_Project_ProjectId",
                table: "Dispatch",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleReturn_Project_ProjectId",
                table: "SaleReturn",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispatch_Project_ProjectId",
                table: "Dispatch");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleReturn_Project_ProjectId",
                table: "SaleReturn");

            migrationBuilder.DropIndex(
                name: "IX_SaleReturn_ProjectId",
                table: "SaleReturn");

            migrationBuilder.DropIndex(
                name: "IX_Dispatch_ProjectId",
                table: "Dispatch");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "SaleReturn");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Dispatch");

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "SaleReturnDetail",
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
                name: "FK_SaleReturnDetail_Project_ProjectId",
                table: "SaleReturnDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
