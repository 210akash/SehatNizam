using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class DeliveryTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTerms",
                table: "PurchaseOrder");

            migrationBuilder.AddColumn<long>(
                name: "DeliveryTermsId",
                table: "PurchaseOrder",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "DeliveryTerms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryTerms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryTerms_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryTerms_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_DeliveryTermsId",
                table: "PurchaseOrder",
                column: "DeliveryTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTerms_CompanyId",
                table: "DeliveryTerms",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTerms_CreatedById",
                table: "DeliveryTerms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTerms_ModifiedById",
                table: "DeliveryTerms",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_DeliveryTerms_DeliveryTermsId",
                table: "PurchaseOrder",
                column: "DeliveryTermsId",
                principalTable: "DeliveryTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_DeliveryTerms_DeliveryTermsId",
                table: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "DeliveryTerms");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_DeliveryTermsId",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "DeliveryTermsId",
                table: "PurchaseOrder");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTerms",
                table: "PurchaseOrder",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
