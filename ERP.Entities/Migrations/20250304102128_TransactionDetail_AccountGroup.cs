using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class TransactionDetail_AccountGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetail_Account_AccountId",
                table: "TransactionDetail");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "TransactionDetail",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "AccountGroupId",
                table: "TransactionDetail",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroup",
                table: "Account",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDetail_AccountGroupId",
                table: "TransactionDetail",
                column: "AccountGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetail_Account_AccountId",
                table: "TransactionDetail",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetail_AccountGroup_AccountGroupId",
                table: "TransactionDetail",
                column: "AccountGroupId",
                principalTable: "AccountGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetail_Account_AccountId",
                table: "TransactionDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDetail_AccountGroup_AccountGroupId",
                table: "TransactionDetail");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDetail_AccountGroupId",
                table: "TransactionDetail");

            migrationBuilder.DropColumn(
                name: "AccountGroupId",
                table: "TransactionDetail");

            migrationBuilder.DropColumn(
                name: "IsGroup",
                table: "Account");

            migrationBuilder.AlterColumn<long>(
                name: "AccountId",
                table: "TransactionDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDetail_Account_AccountId",
                table: "TransactionDetail",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
