using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class SaleMaterialReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleMaterialReturn",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    SaleMaterialId = table.Column<long>(type: "bigint", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint", nullable: true),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleMaterialReturn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_AspNetUsers_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_SaleMaterial_SaleMaterialId",
                        column: x => x.SaleMaterialId,
                        principalTable: "SaleMaterial",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturn_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleMaterialReturnDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleMaterialReturnId = table.Column<long>(type: "bigint", nullable: false),
                    SaleMaterialDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_SaleMaterialReturnDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturnDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturnDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturnDetail_SaleMaterialDetail_SaleMaterialDetailId",
                        column: x => x.SaleMaterialDetailId,
                        principalTable: "SaleMaterialDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleMaterialReturnDetail_SaleMaterialReturn_SaleMaterialReturnId",
                        column: x => x.SaleMaterialReturnId,
                        principalTable: "SaleMaterialReturn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_ApprovedById",
                table: "SaleMaterialReturn",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_CreatedById",
                table: "SaleMaterialReturn",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_ModifiedById",
                table: "SaleMaterialReturn",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_ProcessedById",
                table: "SaleMaterialReturn",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_ProjectId",
                table: "SaleMaterialReturn",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_SaleMaterialId",
                table: "SaleMaterialReturn",
                column: "SaleMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturn_StatusId",
                table: "SaleMaterialReturn",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturnDetail_CreatedById",
                table: "SaleMaterialReturnDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturnDetail_ModifiedById",
                table: "SaleMaterialReturnDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturnDetail_SaleMaterialDetailId",
                table: "SaleMaterialReturnDetail",
                column: "SaleMaterialDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleMaterialReturnDetail_SaleMaterialReturnId",
                table: "SaleMaterialReturnDetail",
                column: "SaleMaterialReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleMaterialReturnDetail");

            migrationBuilder.DropTable(
                name: "SaleMaterialReturn");
        }
    }
}
