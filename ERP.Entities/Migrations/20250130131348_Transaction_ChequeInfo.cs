using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Transaction_ChequeInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChequeClearDate",
                table: "Transaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChequeDate",
                table: "Transaction",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChequeNo",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChequeTitle",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaidReceiveBy",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChequeClearDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ChequeDate",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ChequeNo",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "ChequeTitle",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "PaidReceiveBy",
                table: "Transaction");
        }
    }
}
