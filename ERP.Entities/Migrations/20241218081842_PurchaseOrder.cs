using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PurchaseOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditDays",
                table: "Vendor",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TermsConditions",
                table: "Vendor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VendorTypeId",
                table: "Vendor",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProjectId",
                table: "IndentRequest",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "IndentRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusRemarks",
                table: "IndentRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Currency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currency_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Currency_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Currency_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndentRequestId = table.Column<long>(type: "bigint", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_IndentRequest_IndentRequestId",
                        column: x => x.IndentRequestId,
                        principalTable: "IndentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorType",
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
                    table.PrimaryKey("PK_VendorType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_VendorTypeId",
                table: "Vendor",
                column: "VendorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentRequest_ProjectId",
                table: "IndentRequest",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_CompanyId",
                table: "Currency",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_CreatedById",
                table: "Currency",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_ModifiedById",
                table: "Currency",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Document_CreatedById",
                table: "Document",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Document_IndentRequestId",
                table: "Document",
                column: "IndentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ModifiedById",
                table: "Document",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VendorType_CompanyId",
                table: "VendorType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorType_CreatedById",
                table: "VendorType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VendorType_ModifiedById",
                table: "VendorType",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IndentRequest_Project_ProjectId",
                table: "IndentRequest",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_VendorType_VendorTypeId",
                table: "Vendor",
                column: "VendorTypeId",
                principalTable: "VendorType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndentRequest_Project_ProjectId",
                table: "IndentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_VendorType_VendorTypeId",
                table: "Vendor");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "VendorType");

            migrationBuilder.DropIndex(
                name: "IX_Vendor_VendorTypeId",
                table: "Vendor");

            migrationBuilder.DropIndex(
                name: "IX_IndentRequest_ProjectId",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "CreditDays",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "TermsConditions",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "VendorTypeId",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "StatusRemarks",
                table: "IndentRequest");
        }
    }
}
