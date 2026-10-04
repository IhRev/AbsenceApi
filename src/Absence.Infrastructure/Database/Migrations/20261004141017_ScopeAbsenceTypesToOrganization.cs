using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ScopeAbsenceTypesToOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "AbsenceTypes",
                type: "int",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceTypes_OrganizationId_Name",
                table: "AbsenceTypes",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);

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
                name: "IX_AbsenceTypes_OrganizationId_Name",
                table: "AbsenceTypes");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "AbsenceTypes");
        }
    }
}
