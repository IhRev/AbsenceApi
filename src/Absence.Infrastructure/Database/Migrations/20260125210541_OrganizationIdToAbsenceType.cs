using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationIdToAbsenceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEvents_AbsenceEventTypes_AbsenceEventTypeId",
                table: "AbsenceEvents");

            migrationBuilder.DropTable(
                name: "AbsenceEventTypes");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceEvents_AbsenceEventTypeId",
                table: "AbsenceEvents");

            migrationBuilder.RenameColumn(
                name: "AbsenceEventTypeId",
                table: "AbsenceEvents",
                newName: "AbsenceEventType");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AbsenceTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceTypes_OrganizationId",
                table: "AbsenceTypes",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceTypes_Organizations_OrganizationId",
                table: "AbsenceTypes",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceTypes_Organizations_OrganizationId",
                table: "AbsenceTypes");

            migrationBuilder.DropIndex(
                name: "IX_AbsenceTypes_OrganizationId",
                table: "AbsenceTypes");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AbsenceTypes");

            migrationBuilder.RenameColumn(
                name: "AbsenceEventType",
                table: "AbsenceEvents",
                newName: "AbsenceEventTypeId");

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
