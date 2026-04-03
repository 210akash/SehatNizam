using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "LabOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LabOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "LabOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LabOrder",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "LabOrder",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "LabOrder",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "LabOrder",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_CreatedById",
                table: "LabOrder",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabOrder_ModifiedById",
                table: "LabOrder",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_AspNetUsers_CreatedById",
                table: "LabOrder",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrder_AspNetUsers_ModifiedById",
                table: "LabOrder",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_AspNetUsers_CreatedById",
                table: "LabOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_LabOrder_AspNetUsers_ModifiedById",
                table: "LabOrder");

            migrationBuilder.DropIndex(
                name: "IX_LabOrder_CreatedById",
                table: "LabOrder");

            migrationBuilder.DropIndex(
                name: "IX_LabOrder_ModifiedById",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "LabOrder");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "LabOrder");
        }
    }
}
