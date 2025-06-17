using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class HolidayFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Holidays");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Holidays",
                newName: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Holidays",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "Holidays",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
