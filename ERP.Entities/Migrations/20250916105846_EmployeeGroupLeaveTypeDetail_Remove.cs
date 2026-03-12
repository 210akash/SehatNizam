using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeGroupLeaveTypeDetail_Remove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeGroupLeaveTypeDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeGroupLeaveTypeDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeGroupLeaveTypeId = table.Column<long>(type: "bigint", nullable: true),
                    EmployeeLeaveGroupTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoOfLeaves = table.Column<long>(type: "bigint", nullable: false)
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
    }
}
