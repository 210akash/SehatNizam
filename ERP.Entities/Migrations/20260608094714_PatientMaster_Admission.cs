using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PatientMaster_Admission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_City_CityId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_CityId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "CNIC",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PhoneNo",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "SecondaryPhoneNo",
                table: "Patient");

            migrationBuilder.AddColumn<long>(
                name: "PatientMasterId",
                table: "Patient",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PatientMasterId",
                table: "Appointment",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CNIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<long>(type: "bigint", nullable: true),
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
                    table.PrimaryKey("PK_PatientMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientMaster_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMaster_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientMaster_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_PatientMasterId",
                table: "Patient",
                column: "PatientMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientMasterId",
                table: "Appointment",
                column: "PatientMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMaster_CityId",
                table: "PatientMaster",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMaster_CreatedById",
                table: "PatientMaster",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMaster_ModifiedById",
                table: "PatientMaster",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_PatientMaster_PatientMasterId",
                table: "Appointment",
                column: "PatientMasterId",
                principalTable: "PatientMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_PatientMaster_PatientMasterId",
                table: "Patient",
                column: "PatientMasterId",
                principalTable: "PatientMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_PatientMaster_PatientMasterId",
                table: "Appointment");

            migrationBuilder.DropForeignKey(
                name: "FK_Patient_PatientMaster_PatientMasterId",
                table: "Patient");

            migrationBuilder.DropTable(
                name: "PatientMaster");

            migrationBuilder.DropIndex(
                name: "IX_Patient_PatientMasterId",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_PatientMasterId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PatientMasterId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PatientMasterId",
                table: "Appointment");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Patient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CNIC",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CityId",
                table: "Patient",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNo",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryPhoneNo",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patient_CityId",
                table: "Patient",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_City_CityId",
                table: "Patient",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
