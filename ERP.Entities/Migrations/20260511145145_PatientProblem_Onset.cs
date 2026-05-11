using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP.Entities.Migrations
{
    public partial class PatientProblem_Onset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Onset",
                table: "PatientProblem",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Onset",
                table: "PatientProblem");
        }
    }
}
