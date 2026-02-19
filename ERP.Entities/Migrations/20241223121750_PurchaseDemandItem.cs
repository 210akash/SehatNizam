using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PurchaseDemandItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_IndentRequest_IndentRequestId",
                table: "PurchaseDemand");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemandDetail_IndentRequestDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_IndentRequestId",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "IndentRequestId",
                table: "PurchaseDemand");

            migrationBuilder.RenameColumn(
                name: "IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDemandDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                newName: "IX_PurchaseDemandDetail_ItemId");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "PurchaseDemandDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "PurchaseDemandDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IndentTypeId",
                table: "PurchaseDemand",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "PurchaseDemand",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_DepartmentId",
                table: "PurchaseDemandDetail",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_ProjectId",
                table: "PurchaseDemandDetail",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_IndentTypeId",
                table: "PurchaseDemand",
                column: "IndentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_StoreId",
                table: "PurchaseDemand",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_IndentType_IndentTypeId",
                table: "PurchaseDemand",
                column: "IndentTypeId",
                principalTable: "IndentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_Store_StoreId",
                table: "PurchaseDemand",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemandDetail_Department_DepartmentId",
                table: "PurchaseDemandDetail",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemandDetail_Item_ItemId",
                table: "PurchaseDemandDetail",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemandDetail_Project_ProjectId",
                table: "PurchaseDemandDetail",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_IndentType_IndentTypeId",
                table: "PurchaseDemand");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemand_Store_StoreId",
                table: "PurchaseDemand");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemandDetail_Department_DepartmentId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemandDetail_Item_ItemId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDemandDetail_Project_ProjectId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemandDetail_DepartmentId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemandDetail_ProjectId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_IndentTypeId",
                table: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDemand_StoreId",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseDemandDetail");

            migrationBuilder.DropColumn(
                name: "IndentTypeId",
                table: "PurchaseDemand");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "PurchaseDemand");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "PurchaseDemandDetail",
                newName: "IndentRequestDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDemandDetail_ItemId",
                table: "PurchaseDemandDetail",
                newName: "IX_PurchaseDemandDetail_IndentRequestDetailId");

            migrationBuilder.AddColumn<long>(
                name: "IndentRequestId",
                table: "PurchaseDemand",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_IndentRequestId",
                table: "PurchaseDemand",
                column: "IndentRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemand_IndentRequest_IndentRequestId",
                table: "PurchaseDemand",
                column: "IndentRequestId",
                principalTable: "IndentRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDemandDetail_IndentRequestDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                column: "IndentRequestDetailId",
                principalTable: "IndentRequestDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
