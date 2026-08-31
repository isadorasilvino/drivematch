using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveMatch.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonCheckInToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckInToken",
                table: "lessons",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTokenExpiresAt",
                table: "lessons",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInToken",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "CheckInTokenExpiresAt",
                table: "lessons");
        }
    }
}
