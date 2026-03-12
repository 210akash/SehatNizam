using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Accountheadflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountHeadId",
                table: "AccountCategory",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountFlowId",
                table: "Account",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountFlow",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_AccountFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountFlow_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFlow_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountFlow_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHead",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_AccountHead", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHead_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHead_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHead_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategory_AccountHeadId",
                table: "AccountCategory",
                column: "AccountHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountFlowId",
                table: "Account",
                column: "AccountFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFlow_CompanyId",
                table: "AccountFlow",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFlow_CreatedById",
                table: "AccountFlow",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFlow_ModifiedById",
                table: "AccountFlow",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHead_CompanyId",
                table: "AccountHead",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHead_CreatedById",
                table: "AccountHead",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHead_ModifiedById",
                table: "AccountHead",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AccountFlow_AccountFlowId",
                table: "Account",
                column: "AccountFlowId",
                principalTable: "AccountFlow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCategory_AccountHead_AccountHeadId",
                table: "AccountCategory",
                column: "AccountHeadId",
                principalTable: "AccountHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_AccountFlow_AccountFlowId",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountCategory_AccountHead_AccountHeadId",
                table: "AccountCategory");

            migrationBuilder.DropTable(
                name: "AccountFlow");

            migrationBuilder.DropTable(
                name: "AccountHead");

            migrationBuilder.DropIndex(
                name: "IX_AccountCategory_AccountHeadId",
                table: "AccountCategory");

            migrationBuilder.DropIndex(
                name: "IX_Account_AccountFlowId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "AccountHeadId",
                table: "AccountCategory");

            migrationBuilder.DropColumn(
                name: "AccountFlowId",
                table: "Account");
        }
    }
}
