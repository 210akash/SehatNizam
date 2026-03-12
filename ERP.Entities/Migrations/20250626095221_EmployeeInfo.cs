using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmployeeOvertimeRateId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HrCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResigned",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastCompany",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelevantExperience",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResignDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalWorkExperience",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeOvertimeRate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedById1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeOvertimeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeOvertimeRate_AspNetUsers_CreatedById1",
                        column: x => x.CreatedById1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeOvertimeRate_AspNetUsers_ModifiedById1",
                        column: x => x.ModifiedById1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeOvertimeRateId",
                table: "AspNetUsers",
                column: "EmployeeOvertimeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_CreatedById1",
                table: "EmployeeOvertimeRate",
                column: "CreatedById1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOvertimeRate_ModifiedById1",
                table: "EmployeeOvertimeRate",
                column: "ModifiedById1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeOvertimeRate_EmployeeOvertimeRateId",
                table: "AspNetUsers",
                column: "EmployeeOvertimeRateId",
                principalTable: "EmployeeOvertimeRate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeOvertimeRate_EmployeeOvertimeRateId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "EmployeeOvertimeRate");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeOvertimeRateId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeOvertimeRateId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HrCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsResigned",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastCompany",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RelevantExperience",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResignDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TotalWorkExperience",
                table: "AspNetUsers");
        }
    }
}
