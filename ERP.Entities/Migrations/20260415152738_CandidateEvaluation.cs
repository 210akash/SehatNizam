using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class CandidateEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateEvaluationCategory",
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
                    table.PrimaryKey("PK_CandidateEvaluationCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluationCategory_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluationCategory_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateScoringScale",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateScoringScale", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateEvaluation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewHistoryId = table.Column<long>(type: "bigint", nullable: false),
                    CandidateScoringScaleId = table.Column<long>(type: "bigint", nullable: false),
                    CandidateEvaluationCategoryId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_CandidateEvaluation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluation_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluation_AspNetUsers_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluation_CandidateEvaluationCategory_CandidateEvaluationCategoryId",
                        column: x => x.CandidateEvaluationCategoryId,
                        principalTable: "CandidateEvaluationCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluation_CandidateScoringScale_CandidateScoringScaleId",
                        column: x => x.CandidateScoringScaleId,
                        principalTable: "CandidateScoringScale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateEvaluation_InterviewHistory_InterviewHistoryId",
                        column: x => x.InterviewHistoryId,
                        principalTable: "InterviewHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_CandidateEvaluationCategoryId",
                table: "CandidateEvaluation",
                column: "CandidateEvaluationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_CandidateScoringScaleId",
                table: "CandidateEvaluation",
                column: "CandidateScoringScaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_CreatedById",
                table: "CandidateEvaluation",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_InterviewHistoryId",
                table: "CandidateEvaluation",
                column: "InterviewHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluation_ModifiedById",
                table: "CandidateEvaluation",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluationCategory_CreatedById",
                table: "CandidateEvaluationCategory",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEvaluationCategory_ModifiedById",
                table: "CandidateEvaluationCategory",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateEvaluation");

            migrationBuilder.DropTable(
                name: "CandidateEvaluationCategory");

            migrationBuilder.DropTable(
                name: "CandidateScoringScale");
        }
    }
}
