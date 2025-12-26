using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireZ.Migrations
{
    /// <inheritdoc />
    public partial class ResumeSchemaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Resumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ResumeFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ResumeFeedbacks");
        }
    }
}
