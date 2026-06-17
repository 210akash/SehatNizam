using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class DischargeCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppointmentId",
                table: "BloodRequest",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AppointmentId",
                table: "BloodDonation",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DischargeCertificate",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: false),
                    OperationDeliveryDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hopi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExaminationAndFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvestigationsResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Procedure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurgeonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperativeFindings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionAtDischarge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentAdvisedAtDischarge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProposedFollowUpDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DietAndInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DischargeDoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DischargeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_DischargeCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DischargeCertificate_Admission_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DischargeCertificate_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DischargeCertificate_AspNetUsers_DischargeDoctorId",
                        column: x => x.DischargeDoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DischargeCertificate_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_AppointmentId",
                table: "BloodRequest",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_AppointmentId",
                table: "BloodDonation",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeCertificate_AdmissionId",
                table: "DischargeCertificate",
                column: "AdmissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DischargeCertificate_CreatedById",
                table: "DischargeCertificate",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeCertificate_DischargeDoctorId",
                table: "DischargeCertificate",
                column: "DischargeDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DischargeCertificate_ModifiedById",
                table: "DischargeCertificate",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodDonation_Appointment_AppointmentId",
                table: "BloodDonation",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequest_Appointment_AppointmentId",
                table: "BloodRequest",
                column: "AppointmentId",
                principalTable: "Appointment",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodDonation_Appointment_AppointmentId",
                table: "BloodDonation");

            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequest_Appointment_AppointmentId",
                table: "BloodRequest");

            migrationBuilder.DropTable(
                name: "DischargeCertificate");

            migrationBuilder.DropIndex(
                name: "IX_BloodRequest_AppointmentId",
                table: "BloodRequest");

            migrationBuilder.DropIndex(
                name: "IX_BloodDonation_AppointmentId",
                table: "BloodDonation");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "BloodRequest");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "BloodDonation");
        }
    }
}
