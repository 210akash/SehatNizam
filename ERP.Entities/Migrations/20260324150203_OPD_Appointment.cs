using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class OPD_Appointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrderReturn_Order_OrderId",
                table: "ShopOrderReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrderReturnDetail_OrderItems_OrderItemsId",
                table: "ShopOrderReturnDetail");

            migrationBuilder.RenameColumn(
                name: "OrderItemsId",
                table: "ShopOrderReturnDetail",
                newName: "ShopOrderItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrderReturnDetail_OrderItemsId",
                table: "ShopOrderReturnDetail",
                newName: "IX_ShopOrderReturnDetail_ShopOrderItemsId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "ShopOrderReturn",
                newName: "ShopOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrderReturn_OrderId",
                table: "ShopOrderReturn",
                newName: "IX_ShopOrderReturn_ShopOrderId");

            migrationBuilder.CreateTable(
                name: "AppointmentStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppointmentStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentStatus_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentStatus_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AppointmentType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOrderType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_LabOrderType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrderType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabOrderType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriorityLevel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PriorityLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriorityLevel_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriorityLevel_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriorityLevel_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RadiologyType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SugarType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_SugarType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SugarType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SugarType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SugarType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TriageCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_TriageCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriageCategory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriageCategory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TriagePriority",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_TriagePriority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TriagePriority_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriagePriority_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TriagePriority_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_VisitType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitType_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitType_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitType_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppointmentTypeId = table.Column<long>(type: "bigint", nullable: false),
                    PriorityLevelId = table.Column<long>(type: "bigint", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConfirmedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VisitTypeId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppointmentStatusId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentStatus_AppointmentStatusId",
                        column: x => x.AppointmentStatusId,
                        principalTable: "AppointmentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentType_AppointmentTypeId",
                        column: x => x.AppointmentTypeId,
                        principalTable: "AppointmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_AspNetUsers_ConfirmedById",
                        column: x => x.ConfirmedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_PriorityLevel_PriorityLevelId",
                        column: x => x.PriorityLevelId,
                        principalTable: "PriorityLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_VisitType_VisitTypeId",
                        column: x => x.VisitTypeId,
                        principalTable: "VisitType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentAttachment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<long>(type: "bigint", nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_AppointmentAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentAttachment_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentAttachment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentAttachment_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentAttachment_AspNetUsers_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentPayment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    VisitFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentModeId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatusId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_AppointmentPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentPayment_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentPayment_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentPayment_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppointmentPayment_PaymentMode_PaymentModeId",
                        column: x => x.PaymentModeId,
                        principalTable: "PaymentMode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentPayment_Status_PaymentStatusId",
                        column: x => x.PaymentStatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consultation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    Subjective = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Objective = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Assessment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Plan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowUpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_Consultation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consultation_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consultation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultation_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    LabOrderTypeId = table.Column<long>(type: "bigint", nullable: false),
                    StatusId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabOrder_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrder_LabOrderType_LabOrderTypeId",
                        column: x => x.LabOrderTypeId,
                        principalTable: "LabOrderType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabOrder_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientProblem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    Problem = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PatientProblem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientProblem_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientProblem_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientProblem_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientProblem_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    DrugName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrugCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_Prescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescription_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescription_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescription_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyOrder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    RadiologyTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_RadiologyOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyOrder_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrder_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrder_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrder_RadiologyType_RadiologyTypeId",
                        column: x => x.RadiologyTypeId,
                        principalTable: "RadiologyType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadiologyOrder_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Triage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NurseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pulse = table.Column<int>(type: "int", nullable: true),
                    SystolicBp = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiastolicBp = table.Column<int>(type: "int", nullable: true),
                    Spo2 = table.Column<int>(type: "int", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HeightFeet = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HeightInches = table.Column<int>(type: "int", nullable: true),
                    HeightCm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Bmi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BloodSugar = table.Column<int>(type: "int", nullable: true),
                    SugarTypeId = table.Column<long>(type: "bigint", nullable: false),
                    TriagePriorityId = table.Column<long>(type: "bigint", nullable: false),
                    ChiefComplaint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Medications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriageScore = table.Column<int>(type: "int", nullable: false),
                    TriageCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    TakenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_Triage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Triage_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Triage_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_AspNetUsers_NurseId",
                        column: x => x.NurseId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Triage_SugarType_SugarTypeId",
                        column: x => x.SugarTypeId,
                        principalTable: "SugarType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_TriageCategory_TriageCategoryId",
                        column: x => x.TriageCategoryId,
                        principalTable: "TriageCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Triage_TriagePriority_TriagePriorityId",
                        column: x => x.TriagePriorityId,
                        principalTable: "TriagePriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_AppointmentStatusId",
                table: "Appointment",
                column: "AppointmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_AppointmentTypeId",
                table: "Appointment",
                column: "AppointmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ConfirmedById",
                table: "Appointment",
                column: "ConfirmedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_CreatedById",
                table: "Appointment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DepartmentId",
                table: "Appointment",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId",
                table: "Appointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ModifiedById",
                table: "Appointment",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PriorityLevelId",
                table: "Appointment",
                column: "PriorityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_VisitTypeId",
                table: "Appointment",
                column: "VisitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachment_AppointmentId",
                table: "AppointmentAttachment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachment_CreatedById",
                table: "AppointmentAttachment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachment_ModifiedById",
                table: "AppointmentAttachment",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAttachment_PatientId1",
                table: "AppointmentAttachment",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_AppointmentId",
                table: "AppointmentPayment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_CreatedById",
                table: "AppointmentPayment",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_ModifiedById",
                table: "AppointmentPayment",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_PaymentModeId",
                table: "AppointmentPayment",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentPayment_PaymentStatusId",
                table: "AppointmentPayment",
                column: "PaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatus_CreatedById",
                table: "AppointmentStatus",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentStatus_ModifiedById",
                table: "AppointmentStatus",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentType_CompanyId",
                table: "AppointmentType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentType_CreatedById",
                table: "AppointmentType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentType_ModifiedById",
                table: "AppointmentType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_AppointmentId",
                table: "Consultation",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_CreatedById",
                table: "Consultation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_ModifiedById",
                table: "Consultation",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Consultation_StatusId",
                table: "Consultation",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_AppointmentId",
                table: "LabOrder",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_LabOrderTypeId",
                table: "LabOrder",
                column: "LabOrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_StatusId",
                table: "LabOrder",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderType_CreatedById",
                table: "LabOrderType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderType_ModifiedById",
                table: "LabOrderType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProblem_AppointmentId",
                table: "PatientProblem",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProblem_CreatedById",
                table: "PatientProblem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProblem_ModifiedById",
                table: "PatientProblem",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProblem_StatusId",
                table: "PatientProblem",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_AppointmentId",
                table: "Prescription",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_CreatedById",
                table: "Prescription",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_ModifiedById",
                table: "Prescription",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityLevel_CompanyId",
                table: "PriorityLevel",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityLevel_CreatedById",
                table: "PriorityLevel",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityLevel_ModifiedById",
                table: "PriorityLevel",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrder_AppointmentId",
                table: "RadiologyOrder",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrder_CreatedById",
                table: "RadiologyOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrder_ModifiedById",
                table: "RadiologyOrder",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrder_RadiologyTypeId",
                table: "RadiologyOrder",
                column: "RadiologyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrder_StatusId",
                table: "RadiologyOrder",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyType_CompanyId",
                table: "RadiologyType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyType_CreatedById",
                table: "RadiologyType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyType_ModifiedById",
                table: "RadiologyType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SugarType_CompanyId",
                table: "SugarType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SugarType_CreatedById",
                table: "SugarType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SugarType_ModifiedById",
                table: "SugarType",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_AppointmentId",
                table: "Triage",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_CreatedById",
                table: "Triage",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_ModifiedById",
                table: "Triage",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_NurseId",
                table: "Triage",
                column: "NurseId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_PatientId",
                table: "Triage",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_SugarTypeId",
                table: "Triage",
                column: "SugarTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_TriageCategoryId",
                table: "Triage",
                column: "TriageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Triage_TriagePriorityId",
                table: "Triage",
                column: "TriagePriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_TriageCategory_CreatedById",
                table: "TriageCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TriageCategory_ModifiedById",
                table: "TriageCategory",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_CompanyId",
                table: "TriagePriority",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_CreatedById",
                table: "TriagePriority",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TriagePriority_ModifiedById",
                table: "TriagePriority",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitType_CompanyId",
                table: "VisitType",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitType_CreatedById",
                table: "VisitType",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VisitType_ModifiedById",
                table: "VisitType",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrderReturn_ShopOrder_ShopOrderId",
                table: "ShopOrderReturn",
                column: "ShopOrderId",
                principalTable: "ShopOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrderReturnDetail_ShopOrderItems_ShopOrderItemsId",
                table: "ShopOrderReturnDetail",
                column: "ShopOrderItemsId",
                principalTable: "ShopOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrderReturn_ShopOrder_ShopOrderId",
                table: "ShopOrderReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrderReturnDetail_ShopOrderItems_ShopOrderItemsId",
                table: "ShopOrderReturnDetail");

            migrationBuilder.DropTable(
                name: "AppointmentAttachment");

            migrationBuilder.DropTable(
                name: "AppointmentPayment");

            migrationBuilder.DropTable(
                name: "Consultation");

            migrationBuilder.DropTable(
                name: "LabOrder");

            migrationBuilder.DropTable(
                name: "PatientProblem");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "RadiologyOrder");

            migrationBuilder.DropTable(
                name: "Triage");

            migrationBuilder.DropTable(
                name: "LabOrderType");

            migrationBuilder.DropTable(
                name: "RadiologyType");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "SugarType");

            migrationBuilder.DropTable(
                name: "TriageCategory");

            migrationBuilder.DropTable(
                name: "TriagePriority");

            migrationBuilder.DropTable(
                name: "AppointmentStatus");

            migrationBuilder.DropTable(
                name: "AppointmentType");

            migrationBuilder.DropTable(
                name: "PriorityLevel");

            migrationBuilder.DropTable(
                name: "VisitType");

            migrationBuilder.RenameColumn(
                name: "ShopOrderItemsId",
                table: "ShopOrderReturnDetail",
                newName: "OrderItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrderReturnDetail_ShopOrderItemsId",
                table: "ShopOrderReturnDetail",
                newName: "IX_ShopOrderReturnDetail_OrderItemsId");

            migrationBuilder.RenameColumn(
                name: "ShopOrderId",
                table: "ShopOrderReturn",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopOrderReturn_ShopOrderId",
                table: "ShopOrderReturn",
                newName: "IX_ShopOrderReturn_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrderReturn_Order_OrderId",
                table: "ShopOrderReturn",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrderReturnDetail_OrderItems_OrderItemsId",
                table: "ShopOrderReturnDetail",
                column: "OrderItemsId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
