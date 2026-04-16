using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class InterviewHistory_CandidateEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateEvaluation_Interview_InterviewId",
                table: "CandidateEvaluation");

            migrationBuilder.DropIndex(
                name: "IX_CandidateEvaluation_InterviewId",
                table: "CandidateEvaluation");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "CandidateEvaluation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InterviewId",
                table: "CandidateEvaluation",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_InterviewId",
                table: "CandidateEvaluation",
                column: "InterviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateEvaluation_Interview_InterviewId",
                table: "CandidateEvaluation",
                column: "InterviewId",
                principalTable: "Interview",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
