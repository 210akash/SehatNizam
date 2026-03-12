using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class hr_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocument_AspNetUsers_EmployeeId",
                table: "EmployeeDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeaveGroup_EmployeeLeaveType_EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeaveGroup_EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup");

            migrationBuilder.DropColumn(
                name: "EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup");

            migrationBuilder.CreateTable(
                name: "EmployeeLeaveGroupType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoOfLeaves = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLeaveGroupId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeLeaveGroupType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveGroupType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveGroupType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveGroupType_EmployeeLeaveGroup_EmployeeLeaveGroupId",
                        column: x => x.EmployeeLeaveGroupId,
                        principalTable: "EmployeeLeaveGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveGroupType_EmployeeLeaveType_EmployeeLeaveTypeId",
                        column: x => x.EmployeeLeaveTypeId,
                        principalTable: "EmployeeLeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveGroupType_CreatedById",
                table: "EmployeeLeaveGroupType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveGroupType_EmployeeLeaveGroupId",
                table: "EmployeeLeaveGroupType",
                column: "EmployeeLeaveGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveGroupType_EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroupType",
                column: "EmployeeLeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveGroupType_ModifiedById",
                table: "EmployeeLeaveGroupType",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocument_AspNetUsers_EmployeeId",
                table: "EmployeeDocument",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeDocument_AspNetUsers_EmployeeId",
                table: "EmployeeDocument");

            migrationBuilder.DropTable(
                name: "EmployeeLeaveGroupType");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveGroup_EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup",
                column: "EmployeeLeaveTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeDocument_AspNetUsers_EmployeeId",
                table: "EmployeeDocument",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeaveGroup_EmployeeLeaveType_EmployeeLeaveTypeId",
                table: "EmployeeLeaveGroup",
                column: "EmployeeLeaveTypeId",
                principalTable: "EmployeeLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
