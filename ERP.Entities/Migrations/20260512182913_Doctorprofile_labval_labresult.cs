using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Doctorprofile_labval_labresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_Appointment_AppointmentId",
                table: "LabOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrder_Appointment_AppointmentId",
                table: "RadiologyOrder");

            migrationBuilder.DropColumn(
                name: "CustomFieldsSchema",
                table: "LabOrderType");

            migrationBuilder.DropColumn(
                name: "DoctorShare",
                table: "DoctorServiceFee");

            migrationBuilder.DropColumn(
                name: "FixedAmount",
                table: "DoctorServiceFee");

            migrationBuilder.DropColumn(
                name: "HospitalShare",
                table: "DoctorServiceFee");

            migrationBuilder.RenameColumn(
                name: "Percentage",
                table: "DoctorServiceFee",
                newName: "DoctorPercentage");

            migrationBuilder.AlterColumn<long>(
                name: "AppointmentId",
                table: "RadiologyOrder",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AppointmentId",
                table: "LabOrder",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "DoctorProfileId",
                table: "DoctorServiceFee",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoctorProfile",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PMDCNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsultationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HospitalPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsAvailableForOPD = table.Column<bool>(type: "bit", nullable: false),
                    IsAvailableForIPD = table.Column<bool>(type: "bit", nullable: false),
                    CustomFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_DoctorProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorProfile_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorProfile_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorProfile_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTestVariable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaleMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaleMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FemaleMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FemaleMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasGenderRange = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_LabTestVariable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestVariable_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestVariable_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestVariable_LabOrderType_LabOrderTypeId",
                        column: x => x.LabOrderTypeId,
                        principalTable: "LabOrderType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabResult",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabOrderId = table.Column<long>(type: "bigint", nullable: false),
                    LabTestVariableId = table.Column<long>(type: "bigint", nullable: false),
                    ResultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_LabResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResult_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_LabOrder_LabOrderId",
                        column: x => x.LabOrderId,
                        principalTable: "LabOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabResult_LabTestVariable_LabTestVariableId",
                        column: x => x.LabTestVariableId,
                        principalTable: "LabTestVariable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorServiceFee_DoctorProfileId",
                table: "DoctorServiceFee",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_CreatedById",
                table: "DoctorProfile",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_DoctorId",
                table: "DoctorProfile",
                column: "DoctorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_ModifiedById",
                table: "DoctorProfile",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_CreatedById",
                table: "LabResult",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_LabOrderId",
                table: "LabResult",
                column: "LabOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_LabTestVariableId",
                table: "LabResult",
                column: "LabTestVariableId");

            migrationBuilder.CreateIndex(
                name: "IX_LabResult_ModifiedById",
                table: "LabResult",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariable_CreatedById",
                table: "LabTestVariable",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariable_LabOrderTypeId",
                table: "LabTestVariable",
                column: "LabOrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestVariable_ModifiedById",
                table: "LabTestVariable",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorServiceFee_DoctorProfile_DoctorProfileId",
                table: "DoctorServiceFee",
                column: "DoctorProfileId",
                principalTable: "DoctorProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_Appointment_AppointmentId",
                table: "LabOrder",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrder_Appointment_AppointmentId",
                table: "RadiologyOrder",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorServiceFee_DoctorProfile_DoctorProfileId",
                table: "DoctorServiceFee");

            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_Appointment_AppointmentId",
                table: "LabOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrder_Appointment_AppointmentId",
                table: "RadiologyOrder");

            migrationBuilder.DropTable(
                name: "DoctorProfile");

            migrationBuilder.DropTable(
                name: "LabResult");

            migrationBuilder.DropTable(
                name: "LabTestVariable");

            migrationBuilder.DropIndex(
                name: "IX_DoctorServiceFee_DoctorProfileId",
                table: "DoctorServiceFee");

            migrationBuilder.DropColumn(
                name: "DoctorProfileId",
                table: "DoctorServiceFee");

            migrationBuilder.RenameColumn(
                name: "DoctorPercentage",
                table: "DoctorServiceFee",
                newName: "Percentage");

            migrationBuilder.AlterColumn<long>(
                name: "AppointmentId",
                table: "RadiologyOrder",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomFieldsSchema",
                table: "LabOrderType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AppointmentId",
                table: "LabOrder",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DoctorShare",
                table: "DoctorServiceFee",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FixedAmount",
                table: "DoctorServiceFee",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HospitalShare",
                table: "DoctorServiceFee",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_Appointment_AppointmentId",
                table: "LabOrder",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrder_Appointment_AppointmentId",
                table: "RadiologyOrder",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
