using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveMatch.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailabilityLessonDurationAndBreak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakDurationMinutes",
                table: "availabilities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LessonDurationMinutes",
                table: "availabilities",
                type: "integer",
                nullable: false,
                defaultValue: 60);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakDurationMinutes",
                table: "availabilities");

            migrationBuilder.DropColumn(
                name: "LessonDurationMinutes",
                table: "availabilities");
        }
    }
}
