using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class issuance_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuanceDetails");

            migrationBuilder.CreateTable(
                name: "IssuanceDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssuanceId = table.Column<long>(type: "bigint", nullable: false),
                    IndentRequestDetailId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_IssuanceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuanceDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuanceDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuanceDetail_IndentRequestDetail_IndentRequestDetailId",
                        column: x => x.IndentRequestDetailId,
                        principalTable: "IndentRequestDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssuanceDetail_Issuance_IssuanceId",
                        column: x => x.IssuanceId,
                        principalTable: "Issuance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_CreatedById",
                table: "IssuanceDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_IndentRequestDetailId",
                table: "IssuanceDetail",
                column: "IndentRequestDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_IssuanceId",
                table: "IssuanceDetail",
                column: "IssuanceId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetail_ModifiedById",
                table: "IssuanceDetail",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuanceDetail");

            migrationBuilder.CreateTable(
                name: "IssuanceDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IndentRequestDetailId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IssuanceId = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuanceDetails_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuanceDetails_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuanceDetails_IndentRequestDetail_IndentRequestDetailId",
                        column: x => x.IndentRequestDetailId,
                        principalTable: "IndentRequestDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssuanceDetails_Issuance_IssuanceId",
                        column: x => x.IssuanceId,
                        principalTable: "Issuance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetails_CreatedById",
                table: "IssuanceDetails",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetails_IndentRequestDetailId",
                table: "IssuanceDetails",
                column: "IndentRequestDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetails_IssuanceId",
                table: "IssuanceDetails",
                column: "IssuanceId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuanceDetails_ModifiedById",
                table: "IssuanceDetails",
                column: "ModifiedById");
        }
    }
}
