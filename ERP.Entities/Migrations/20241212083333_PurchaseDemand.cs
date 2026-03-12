using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PurchaseDemand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Priority");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Priority",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IndentTypeId",
                table: "IndentRequest",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "IndentRequest",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IndentType",
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
                    table.PrimaryKey("PK_IndentType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndentType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndentType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndentType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDemand",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    IndentRequestId = table.Column<long>(type: "bigint", nullable: false),
                    PriorityId = table.Column<long>(type: "bigint", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_PurchaseDemand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_IndentRequest_IndentRequestId",
                        column: x => x.IndentRequestId,
                        principalTable: "IndentRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_Priority_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priority",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseDemand_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseDemandDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDemandId = table.Column<long>(type: "bigint", nullable: false),
                    IndentRequestDetailId = table.Column<long>(type: "bigint", nullable: false),
                    Required = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PurchaseDemandDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandDetail_IndentRequestDetail_IndentRequestDetailId",
                        column: x => x.IndentRequestDetailId,
                        principalTable: "IndentRequestDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseDemandDetail_PurchaseDemand_PurchaseDemandId",
                        column: x => x.PurchaseDemandId,
                        principalTable: "PurchaseDemand",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Priority_CompanyId",
                table: "Priority",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentRequest_IndentTypeId",
                table: "IndentRequest",
                column: "IndentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentRequest_StoreId",
                table: "IndentRequest",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentType_CompanyId",
                table: "IndentType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentType_CreatedById",
                table: "IndentType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IndentType_ModifiedById",
                table: "IndentType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_CreatedById",
                table: "PurchaseDemand",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_IndentRequestId",
                table: "PurchaseDemand",
                column: "IndentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_LocationId",
                table: "PurchaseDemand",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_ModifiedById",
                table: "PurchaseDemand",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_PriorityId",
                table: "PurchaseDemand",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemand_StatusId",
                table: "PurchaseDemand",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_CreatedById",
                table: "PurchaseDemandDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_IndentRequestDetailId",
                table: "PurchaseDemandDetail",
                column: "IndentRequestDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_ModifiedById",
                table: "PurchaseDemandDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDemandDetail_PurchaseDemandId",
                table: "PurchaseDemandDetail",
                column: "PurchaseDemandId");

            migrationBuilder.AddForeignKey(
                name: "FK_IndentRequest_IndentType_IndentTypeId",
                table: "IndentRequest",
                column: "IndentTypeId",
                principalTable: "IndentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IndentRequest_Store_StoreId",
                table: "IndentRequest",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Priority_Company_CompanyId",
                table: "Priority",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IndentRequest_IndentType_IndentTypeId",
                table: "IndentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_IndentRequest_Store_StoreId",
                table: "IndentRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Priority_Company_CompanyId",
                table: "Priority");

            migrationBuilder.DropTable(
                name: "IndentType");

            migrationBuilder.DropTable(
                name: "PurchaseDemandDetail");

            migrationBuilder.DropTable(
                name: "PurchaseDemand");

            migrationBuilder.DropIndex(
                name: "IX_Priority_CompanyId",
                table: "Priority");

            migrationBuilder.DropIndex(
                name: "IX_IndentRequest_IndentTypeId",
                table: "IndentRequest");

            migrationBuilder.DropIndex(
                name: "IX_IndentRequest_StoreId",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Priority");

            migrationBuilder.DropColumn(
                name: "IndentTypeId",
                table: "IndentRequest");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "IndentRequest");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Priority",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
