using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropAbsenceEventTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEvents_AbsenceEventTypes_AbsenceEventTypeId",
                table: "AbsenceEvents");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceEvents_AbsenceEventTypeId",
                table: "AbsenceEvents");

            migrationBuilder.DropTable(
                name: "AbsenceEventTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbsenceEventTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsenceEventTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceEvents_AbsenceEventTypeId",
                table: "AbsenceEvents",
                column: "AbsenceEventTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEvents_AbsenceEventTypes_AbsenceEventTypeId",
                table: "AbsenceEvents",
                column: "AbsenceEventTypeId",
                principalTable: "AbsenceEventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
