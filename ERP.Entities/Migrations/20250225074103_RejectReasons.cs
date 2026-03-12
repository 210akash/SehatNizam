using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class RejectReasons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Approved",
                table: "InspectionDetail",
                newName: "Rejected");

            migrationBuilder.AddColumn<long>(
                name: "RejectReasonId",
                table: "InspectionDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "InspectionDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RejectReason",
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
                    table.PrimaryKey("PK_RejectReason", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RejectReason_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RejectReason_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RejectReason_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionDetail_RejectReasonId",
                table: "InspectionDetail",
                column: "RejectReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_RejectReason_CompanyId",
                table: "RejectReason",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RejectReason_CreatedById",
                table: "RejectReason",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RejectReason_ModifiedById",
                table: "RejectReason",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail",
                column: "RejectReasonId",
                principalTable: "RejectReason",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionDetail_RejectReason_RejectReasonId",
                table: "InspectionDetail");

            migrationBuilder.DropTable(
                name: "RejectReason");

            migrationBuilder.DropIndex(
                name: "IX_InspectionDetail_RejectReasonId",
                table: "InspectionDetail");

            migrationBuilder.DropColumn(
                name: "RejectReasonId",
                table: "InspectionDetail");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "InspectionDetail");

            migrationBuilder.RenameColumn(
                name: "Rejected",
                table: "InspectionDetail",
                newName: "Approved");
        }
    }
}
