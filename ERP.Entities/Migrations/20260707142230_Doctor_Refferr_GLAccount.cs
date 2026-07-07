using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class Doctor_Refferr_GLAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountGroupId",
                table: "Referrer",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Referrer",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Referrer",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "AccountGroupId",
                table: "DoctorProfile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "DoctorProfile",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "DoctorProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_AccountGroupId",
                table: "Referrer",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Referrer_AccountId",
                table: "Referrer",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_AccountGroupId",
                table: "DoctorProfile",
                column: "AccountGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfile_AccountId",
                table: "DoctorProfile",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfile_Account_AccountId",
                table: "DoctorProfile",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfile_AccountGroup_AccountGroupId",
                table: "DoctorProfile",
                column: "AccountGroupId",
                principalTable: "AccountGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Referrer_Account_AccountId",
                table: "Referrer",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Referrer_AccountGroup_AccountGroupId",
                table: "Referrer",
                column: "AccountGroupId",
                principalTable: "AccountGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfile_Account_AccountId",
                table: "DoctorProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfile_AccountGroup_AccountGroupId",
                table: "DoctorProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_Referrer_Account_AccountId",
                table: "Referrer");

            migrationBuilder.DropForeignKey(
                name: "FK_Referrer_AccountGroup_AccountGroupId",
                table: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_Referrer_AccountGroupId",
                table: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_Referrer_AccountId",
                table: "Referrer");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfile_AccountGroupId",
                table: "DoctorProfile");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfile_AccountId",
                table: "DoctorProfile");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Referrer");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "DoctorProfile");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "DoctorProfile");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "DoctorProfile");
        }
    }
}
