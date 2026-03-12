using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeGroupLeaveTypeDetail_Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeGroupLeaveTypeDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeLeaveGroupTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeGroupLeaveTypeId = table.Column<long>(type: "bigint", nullable: true),
                    NoOfLeaves = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeGroupLeaveTypeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveTypeDetail_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveTypeDetail_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                        column: x => x.EmployeeGroupLeaveTypeId,
                        principalTable: "EmployeeGroupLeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveTypeDetail_EmployeeLeaveType_EmployeeLeaveTypeId",
                        column: x => x.EmployeeLeaveTypeId,
                        principalTable: "EmployeeLeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveTypeDetail_CreatedById",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveTypeDetail_EmployeeGroupLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "EmployeeGroupLeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveTypeDetail_EmployeeLeaveTypeId",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "EmployeeLeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveTypeDetail_ModifiedById",
                table: "EmployeeGroupLeaveTypeDetail",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeGroupLeaveTypeDetail");
        }
    }
}
