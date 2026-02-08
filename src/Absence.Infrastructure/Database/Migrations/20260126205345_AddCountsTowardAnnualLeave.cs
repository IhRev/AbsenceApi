using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCountsTowardAnnualLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CountsTowardAnnualLeave",
                table: "AbsenceTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountsTowardAnnualLeave",
                table: "AbsenceTypes");
        }
    }
}
