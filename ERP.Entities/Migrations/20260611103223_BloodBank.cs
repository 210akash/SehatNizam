using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class BloodBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Service",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BloodComponentType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShelfLifeDays = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_BloodComponentType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodComponentType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodComponentType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BloodFridge",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodFridge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodFridge_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodFridge_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BloodGroupMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodGroupMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodGroupMaster_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodGroupMaster_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BloodRack",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodFridgeId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_BloodRack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodRack_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodRack_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodRack_BloodFridge_BloodFridgeId",
                        column: x => x.BloodFridgeId,
                        principalTable: "BloodFridge",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodDonor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CNIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BloodGroupMasterId = table.Column<long>(type: "bigint", nullable: true),
                    PatientMasterId = table.Column<long>(type: "bigint", nullable: true),
                    LastDonationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferralReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodDonor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodDonor_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodDonor_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodDonor_BloodGroupMaster_BloodGroupMasterId",
                        column: x => x.BloodGroupMasterId,
                        principalTable: "BloodGroupMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodDonor_PatientMaster_PatientMasterId",
                        column: x => x.PatientMasterId,
                        principalTable: "PatientMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodRequest",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdmissionId = table.Column<long>(type: "bigint", nullable: true),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientCNIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroupMasterId = table.Column<long>(type: "bigint", nullable: false),
                    BloodComponentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodRequest_Admission_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admission",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodRequest_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodRequest_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodRequest_BloodComponentType_BloodComponentTypeId",
                        column: x => x.BloodComponentTypeId,
                        principalTable: "BloodComponentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodRequest_BloodGroupMaster_BloodGroupMasterId",
                        column: x => x.BloodGroupMasterId,
                        principalTable: "BloodGroupMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodDonation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodDonorId = table.Column<long>(type: "bigint", nullable: false),
                    BloodComponentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    BloodGroupMasterId = table.Column<long>(type: "bigint", nullable: true),
                    DonationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScreeningStatus = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodDonation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodDonation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodDonation_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodDonation_BloodComponentType_BloodComponentTypeId",
                        column: x => x.BloodComponentTypeId,
                        principalTable: "BloodComponentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodDonation_BloodDonor_BloodDonorId",
                        column: x => x.BloodDonorId,
                        principalTable: "BloodDonor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodDonation_BloodGroupMaster_BloodGroupMasterId",
                        column: x => x.BloodGroupMasterId,
                        principalTable: "BloodGroupMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodUnit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodDonationId = table.Column<long>(type: "bigint", nullable: true),
                    BloodComponentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    BloodGroupMasterId = table.Column<long>(type: "bigint", nullable: false),
                    Volume = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodFridgeId = table.Column<long>(type: "bigint", nullable: true),
                    BloodRackId = table.Column<long>(type: "bigint", nullable: true),
                    SlotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_BloodUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodUnit_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodUnit_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodComponentType_BloodComponentTypeId",
                        column: x => x.BloodComponentTypeId,
                        principalTable: "BloodComponentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodDonation_BloodDonationId",
                        column: x => x.BloodDonationId,
                        principalTable: "BloodDonation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodFridge_BloodFridgeId",
                        column: x => x.BloodFridgeId,
                        principalTable: "BloodFridge",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodGroupMaster_BloodGroupMasterId",
                        column: x => x.BloodGroupMasterId,
                        principalTable: "BloodGroupMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodRack_BloodRackId",
                        column: x => x.BloodRackId,
                        principalTable: "BloodRack",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodCrossMatch",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodRequestId = table.Column<long>(type: "bigint", nullable: false),
                    BloodUnitId = table.Column<long>(type: "bigint", nullable: false),
                    CrossMatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodCrossMatch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodCrossMatch_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodCrossMatch_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodCrossMatch_BloodRequest_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodCrossMatch_BloodUnit_BloodUnitId",
                        column: x => x.BloodUnitId,
                        principalTable: "BloodUnit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodIssue",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodRequestId = table.Column<long>(type: "bigint", nullable: false),
                    BloodUnitId = table.Column<long>(type: "bigint", nullable: false),
                    BloodCrossMatchId = table.Column<long>(type: "bigint", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_BloodIssue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BloodIssue_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodIssue_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BloodIssue_BloodCrossMatch_BloodCrossMatchId",
                        column: x => x.BloodCrossMatchId,
                        principalTable: "BloodCrossMatch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodIssue_BloodRequest_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BloodIssue_BloodUnit_BloodUnitId",
                        column: x => x.BloodUnitId,
                        principalTable: "BloodUnit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodComponentType_CreatedById",
                table: "BloodComponentType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodComponentType_ModifiedById",
                table: "BloodComponentType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodCrossMatch_BloodRequestId",
                table: "BloodCrossMatch",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodCrossMatch_BloodUnitId",
                table: "BloodCrossMatch",
                column: "BloodUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodCrossMatch_CreatedById",
                table: "BloodCrossMatch",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodCrossMatch_ModifiedById",
                table: "BloodCrossMatch",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_BloodComponentTypeId",
                table: "BloodDonation",
                column: "BloodComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_BloodDonorId",
                table: "BloodDonation",
                column: "BloodDonorId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_BloodGroupMasterId",
                table: "BloodDonation",
                column: "BloodGroupMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_CreatedById",
                table: "BloodDonation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_ModifiedById",
                table: "BloodDonation",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonor_BloodGroupMasterId",
                table: "BloodDonor",
                column: "BloodGroupMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonor_CreatedById",
                table: "BloodDonor",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonor_ModifiedById",
                table: "BloodDonor",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonor_PatientMasterId",
                table: "BloodDonor",
                column: "PatientMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodFridge_CreatedById",
                table: "BloodFridge",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodFridge_ModifiedById",
                table: "BloodFridge",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodGroupMaster_CreatedById",
                table: "BloodGroupMaster",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodGroupMaster_ModifiedById",
                table: "BloodGroupMaster",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssue_BloodCrossMatchId",
                table: "BloodIssue",
                column: "BloodCrossMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssue_BloodRequestId",
                table: "BloodIssue",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssue_BloodUnitId",
                table: "BloodIssue",
                column: "BloodUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssue_CreatedById",
                table: "BloodIssue",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodIssue_ModifiedById",
                table: "BloodIssue",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRack_BloodFridgeId",
                table: "BloodRack",
                column: "BloodFridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRack_CreatedById",
                table: "BloodRack",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRack_ModifiedById",
                table: "BloodRack",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_AdmissionId",
                table: "BloodRequest",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_BloodComponentTypeId",
                table: "BloodRequest",
                column: "BloodComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_BloodGroupMasterId",
                table: "BloodRequest",
                column: "BloodGroupMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_CreatedById",
                table: "BloodRequest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_ModifiedById",
                table: "BloodRequest",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodComponentTypeId",
                table: "BloodUnit",
                column: "BloodComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodDonationId",
                table: "BloodUnit",
                column: "BloodDonationId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodFridgeId",
                table: "BloodUnit",
                column: "BloodFridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodGroupMasterId",
                table: "BloodUnit",
                column: "BloodGroupMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodRackId",
                table: "BloodUnit",
                column: "BloodRackId");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_CreatedById",
                table: "BloodUnit",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_ModifiedById",
                table: "BloodUnit",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodIssue");

            migrationBuilder.DropTable(
                name: "BloodCrossMatch");

            migrationBuilder.DropTable(
                name: "BloodRequest");

            migrationBuilder.DropTable(
                name: "BloodUnit");

            migrationBuilder.DropTable(
                name: "BloodDonation");

            migrationBuilder.DropTable(
                name: "BloodRack");

            migrationBuilder.DropTable(
                name: "BloodComponentType");

            migrationBuilder.DropTable(
                name: "BloodDonor");

            migrationBuilder.DropTable(
                name: "BloodFridge");

            migrationBuilder.DropTable(
                name: "BloodGroupMaster");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Service");
        }
    }
}
