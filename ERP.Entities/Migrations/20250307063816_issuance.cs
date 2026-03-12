using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class issuance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Issuance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IndentRequestId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Issuance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Issuance_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issuance_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Issuance_IndentRequest_IndentRequestId",
                        column: x => x.IndentRequestId,
                        principalTable: "IndentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Issuance_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssuanceDetails",
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_CreatedById",
                table: "Issuance",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_IndentRequestId",
                table: "Issuance",
                column: "IndentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_ModifiedById",
                table: "Issuance",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Issuance_StatusId",
                table: "Issuance",
                column: "StatusId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssuanceDetails");

            migrationBuilder.DropTable(
                name: "Issuance");
        }
    }
}
