using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class ServiceAccount_History : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Account_AccountId",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_AccountGroup_AccountGroupId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_AccountGroupId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_AccountId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Service");

            migrationBuilder.CreateTable(
                name: "ServiceAccount",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    DebitAccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreditAccountId = table.Column<long>(type: "bigint", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ServiceAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAccount_Account_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceAccount_Account_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAccount_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAccount_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAccount_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAccountHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceAccountId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    OldDebitAccountId = table.Column<long>(type: "bigint", nullable: true),
                    OldCreditAccountId = table.Column<long>(type: "bigint", nullable: true),
                    NewDebitAccountId = table.Column<long>(type: "bigint", nullable: true),
                    NewCreditAccountId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ServiceAccountHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAccountHistory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAccountHistory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAccountHistory_ServiceAccount_ServiceAccountId",
                        column: x => x.ServiceAccountId,
                        principalTable: "ServiceAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_CreatedById",
                table: "ServiceAccount",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_CreditAccountId",
                table: "ServiceAccount",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_DebitAccountId",
                table: "ServiceAccount",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_ModifiedById",
                table: "ServiceAccount",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccount_ServiceId",
                table: "ServiceAccount",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccountHistory_CreatedById",
                table: "ServiceAccountHistory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccountHistory_ModifiedById",
                table: "ServiceAccountHistory",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAccountHistory_ServiceAccountId",
                table: "ServiceAccountHistory",
                column: "ServiceAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAccountHistory");

            migrationBuilder.DropTable(
                name: "ServiceAccount");

            migrationBuilder.AddColumn<long>(
                name: "AccountGroupId",
                table: "Service",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Service",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Service",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Service_AccountGroupId",
                table: "Service",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_AccountId",
                table: "Service",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Account_AccountId",
                table: "Service",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_AccountGroup_AccountGroupId",
                table: "Service",
                column: "AccountGroupId",
                principalTable: "AccountGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
