using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class LabTestVariableOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "LabTestVariable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResultType",
                table: "LabTestVariable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAbnormal",
                table: "LabResult",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "LabResult",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "LabResult",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariableName",
                table: "LabResult",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reference",
                table: "LabOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "LabTestVariableOption",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestVariableId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LabTestVariableOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestVariableOption_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestVariableOption_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestVariableOption_LabTestVariable_LabTestVariableId",
                        column: x => x.LabTestVariableId,
                        principalTable: "LabTestVariable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariableOption_CreatedById",
                table: "LabTestVariableOption",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariableOption_LabTestVariableId",
                table: "LabTestVariableOption",
                column: "LabTestVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariableOption_ModifiedById",
                table: "LabTestVariableOption",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestVariableOption");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "LabTestVariable");

            migrationBuilder.DropColumn(
                name: "ResultType",
                table: "LabTestVariable");

            migrationBuilder.DropColumn(
                name: "IsAbnormal",
                table: "LabResult");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "LabResult");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "LabResult");

            migrationBuilder.DropColumn(
                name: "VariableName",
                table: "LabResult");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "LabOrder");

            migrationBuilder.AlterColumn<Guid>(
                name: "DoctorId",
                table: "Appointment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
