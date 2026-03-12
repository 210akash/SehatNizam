using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class EmployeeWorkSiteType_Interview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InterviewId",
                table: "Attachments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeWorkSiteTypeId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeWorkSiteType",
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
                    table.PrimaryKey("PK_EmployeeWorkSiteType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkSiteType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeWorkSiteType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Interview",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    EmployeeEducationId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeDesignationId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false),
                    CurrentSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpectedSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Experience = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalDetail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Interview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interview_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interview_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interview_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_EmployeeDesignation_EmployeeDesignationId",
                        column: x => x.EmployeeDesignationId,
                        principalTable: "EmployeeDesignation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_EmployeeEducation_EmployeeEducationId",
                        column: x => x.EmployeeEducationId,
                        principalTable: "EmployeeEducation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewId = table.Column<long>(type: "bigint", nullable: false),
                    InterviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinAfterDays = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_InterviewHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewHistory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewHistory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewHistory_Interview_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewHistory_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewAttendees",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewHistoryId = table.Column<long>(type: "bigint", nullable: false),
                    AspNetUsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_InterviewAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewAttendees_AspNetUsers_AspNetUsersId",
                        column: x => x.AspNetUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewAttendees_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAttendees_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewAttendees_InterviewHistory_InterviewHistoryId",
                        column: x => x.InterviewHistoryId,
                        principalTable: "InterviewHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_InterviewId",
                table: "Attachments",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeWorkSiteTypeId",
                table: "AspNetUsers",
                column: "EmployeeWorkSiteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWorkSiteType_CreatedById",
                table: "EmployeeWorkSiteType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeWorkSiteType_ModifiedById",
                table: "EmployeeWorkSiteType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_CompanyId",
                table: "Interview",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_CreatedById",
                table: "Interview",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_DepartmentId",
                table: "Interview",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_EmployeeDesignationId",
                table: "Interview",
                column: "EmployeeDesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_EmployeeEducationId",
                table: "Interview",
                column: "EmployeeEducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_ModifiedById",
                table: "Interview",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_StatusId",
                table: "Interview",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttendees_AspNetUsersId",
                table: "InterviewAttendees",
                column: "AspNetUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttendees_CreatedById",
                table: "InterviewAttendees",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttendees_InterviewHistoryId",
                table: "InterviewAttendees",
                column: "InterviewHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewAttendees_ModifiedById",
                table: "InterviewAttendees",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewHistory_CreatedById",
                table: "InterviewHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewHistory_InterviewId",
                table: "InterviewHistory",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewHistory_ModifiedById",
                table: "InterviewHistory",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewHistory_StatusId",
                table: "InterviewHistory",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeWorkSiteType_EmployeeWorkSiteTypeId",
                table: "AspNetUsers",
                column: "EmployeeWorkSiteTypeId",
                principalTable: "EmployeeWorkSiteType",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Interview_InterviewId",
                table: "Attachments",
                column: "InterviewId",
                principalTable: "Interview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeWorkSiteType_EmployeeWorkSiteTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Interview_InterviewId",
                table: "Attachments");

            migrationBuilder.DropTable(
                name: "EmployeeWorkSiteType");

            migrationBuilder.DropTable(
                name: "InterviewAttendees");

            migrationBuilder.DropTable(
                name: "InterviewHistory");

            migrationBuilder.DropTable(
                name: "Interview");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_InterviewId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeWorkSiteTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "EmployeeWorkSiteTypeId",
                table: "AspNetUsers");
        }
    }
}
