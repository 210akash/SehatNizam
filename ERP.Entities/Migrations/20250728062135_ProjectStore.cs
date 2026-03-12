using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ProjectStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProjectStoreId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectStore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<long>(type: "bigint", nullable: false),
                    StoreId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_ProjectStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStore_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectStore_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectStore_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectStore_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProjectStoreId",
                table: "AspNetUsers",
                column: "ProjectStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStore_CreatedById",
                table: "ProjectStore",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStore_ModifiedById",
                table: "ProjectStore",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStore_ProjectId",
                table: "ProjectStore",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStore_StoreId",
                table: "ProjectStore",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ProjectStore_ProjectStoreId",
                table: "AspNetUsers",
                column: "ProjectStoreId",
                principalTable: "ProjectStore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ProjectStore_ProjectStoreId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ProjectStore");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProjectStoreId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProjectStoreId",
                table: "AspNetUsers");
        }
    }
}
