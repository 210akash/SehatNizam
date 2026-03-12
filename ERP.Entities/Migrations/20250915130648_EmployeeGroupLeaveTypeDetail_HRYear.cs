using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeGroupLeaveTypeDetail_HRYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeLeaveGroupType_EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave");

            migrationBuilder.DropTable(
                name: "EmployeeLeaveGroupType");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeave_EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HRYear",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_HRYear", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRYear_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HRYear_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeGroupLeaveType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeLeaveGroupId = table.Column<long>(type: "bigint", nullable: false),
                    HRYearId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeGroupLeaveType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveType_EmployeeLeaveGroup_EmployeeLeaveGroupId",
                        column: x => x.EmployeeLeaveGroupId,
                        principalTable: "EmployeeLeaveGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeGroupLeaveType_HRYear_HRYearId",
                        column: x => x.HRYearId,
                        principalTable: "HRYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_EmployeeLeave_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveType_CreatedById",
                table: "EmployeeGroupLeaveType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveType_EmployeeLeaveGroupId",
                table: "EmployeeGroupLeaveType",
                column: "EmployeeLeaveGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveType_HRYearId",
                table: "EmployeeGroupLeaveType",
                column: "HRYearId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeGroupLeaveType_ModifiedById",
                table: "EmployeeGroupLeaveType",
                column: "ModifiedById");

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

            migrationBuilder.CreateIndex(
                name: "IX_HRYear_CreatedById",
                table: "HRYear",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HRYear_ModifiedById",
                table: "HRYear",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave",
                column: "EmployeeGroupLeaveTypeId",
                principalTable: "EmployeeGroupLeaveType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeave_EmployeeGroupLeaveType_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.DropTable(
                name: "EmployeeGroupLeaveTypeDetail");

            migrationBuilder.DropTable(
                name: "EmployeeGroupLeaveType");

            migrationBuilder.DropTable(
                name: "HRYear");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeave_EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.DropColumn(
                name: "EmployeeGroupLeaveTypeId",
                table: "EmployeeLeave");

            migrationBuilder.CreateTable(
                name: "EmployeeLeaveGroupType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeLeaveGroupId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeLeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoOfLeaves = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveGroupType_EmployeeLeaveType_EmployeeLeaveTypeId",
                        column: x => x.EmployeeLeaveTypeId,
                        principalTable: "EmployeeLeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeave_EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave",
                column: "EmployeeLeaveGroupTypeId");

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
                name: "FK_EmployeeLeave_EmployeeLeaveGroupType_EmployeeLeaveGroupTypeId",
                table: "EmployeeLeave",
                column: "EmployeeLeaveGroupTypeId",
                principalTable: "EmployeeLeaveGroupType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
