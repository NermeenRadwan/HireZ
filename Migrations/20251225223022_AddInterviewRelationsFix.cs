using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireZ.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewRelationsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuestionsJson",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "ScoresJson",
                table: "InterviewSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "InterviewSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResumeId1",
                table: "InterviewSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "InterviewSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InterviewAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewQuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewSessionId = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewQuestions_InterviewSessions_InterviewSessionId",
                        column: x => x.InterviewSessionId,
                        principalTable: "InterviewSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSessions_ResumeId1",
                table: "InterviewSessions",
                column: "ResumeId1");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_InterviewSessionId",
                table: "InterviewQuestions",
                column: "InterviewSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSessions_Resumes_ResumeId1",
                table: "InterviewSessions",
                column: "ResumeId1",
                principalTable: "Resumes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSessions_Resumes_ResumeId1",
                table: "InterviewSessions");

            migrationBuilder.DropTable(
                name: "InterviewAnswers");

            migrationBuilder.DropTable(
                name: "InterviewQuestions");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSessions_ResumeId1",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "ResumeId1",
                table: "InterviewSessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InterviewSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "QuestionsJson",
                table: "InterviewSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScoresJson",
                table: "InterviewSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Analytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}
