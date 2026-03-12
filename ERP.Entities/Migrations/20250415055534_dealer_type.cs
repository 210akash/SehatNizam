using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class dealer_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dealerships_Territories_TerritoryId",
                table: "Dealerships");

            migrationBuilder.AlterColumn<long>(
                name: "TerritoryId",
                table: "Dealerships",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "DealershipTypeId",
                table: "Dealerships",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DealershipType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_DealershipType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealershipType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealershipType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dealerships_DealershipTypeId",
                table: "Dealerships",
                column: "DealershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DealershipType_CreatedById",
                table: "DealershipType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DealershipType_ModifiedById",
                table: "DealershipType",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Dealerships_DealershipType_DealershipTypeId",
                table: "Dealerships",
                column: "DealershipTypeId",
                principalTable: "DealershipType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Dealerships_Territories_TerritoryId",
                table: "Dealerships",
                column: "TerritoryId",
                principalTable: "Territories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dealerships_DealershipType_DealershipTypeId",
                table: "Dealerships");

            migrationBuilder.DropForeignKey(
                name: "FK_Dealerships_Territories_TerritoryId",
                table: "Dealerships");

            migrationBuilder.DropTable(
                name: "DealershipType");

            migrationBuilder.DropIndex(
                name: "IX_Dealerships_DealershipTypeId",
                table: "Dealerships");

            migrationBuilder.DropColumn(
                name: "DealershipTypeId",
                table: "Dealerships");

            migrationBuilder.AlterColumn<long>(
                name: "TerritoryId",
                table: "Dealerships",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dealerships_Territories_TerritoryId",
                table: "Dealerships",
                column: "TerritoryId",
                principalTable: "Territories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
