using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class IGP_Type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BiltyNo",
                table: "IGP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverCnic",
                table: "IGP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverMobileNo",
                table: "IGP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "IGP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IGPTypeId",
                table: "IGP",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleNo",
                table: "IGP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IGPType",
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
                    table.PrimaryKey("PK_IGPType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IGPType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IGPType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IGP_IGPTypeId",
                table: "IGP",
                column: "IGPTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IGPType_CreatedById",
                table: "IGPType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IGPType_ModifiedById",
                table: "IGPType",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_IGP_IGPType_IGPTypeId",
                table: "IGP",
                column: "IGPTypeId",
                principalTable: "IGPType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IGP_IGPType_IGPTypeId",
                table: "IGP");

            migrationBuilder.DropTable(
                name: "IGPType");

            migrationBuilder.DropIndex(
                name: "IX_IGP_IGPTypeId",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "BiltyNo",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "DriverCnic",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "DriverMobileNo",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "IGPTypeId",
                table: "IGP");

            migrationBuilder.DropColumn(
                name: "VehicleNo",
                table: "IGP");
        }
    }
}
