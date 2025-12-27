using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireZ.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewEntities_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSessions_Resumes_ResumeId1",
                table: "InterviewSessions");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSessions_ResumeId1",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "ResumeId1",
                table: "InterviewSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResumeId1",
                table: "InterviewSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_ResumeId1",
                table: "InterviewSessions",
                column: "ResumeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSessions_Resumes_ResumeId1",
                table: "InterviewSessions",
                column: "ResumeId1",
                principalTable: "Resumes",
                principalColumn: "Id");
        }
    }
}
