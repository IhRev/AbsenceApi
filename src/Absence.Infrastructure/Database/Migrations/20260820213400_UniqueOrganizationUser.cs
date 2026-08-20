using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UniqueOrganizationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationUsers_OrganizationId",
                table: "OrganizationUsers");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_OrganizationId_UserId",
                table: "OrganizationUsers",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganizationUsers_OrganizationId_UserId",
                table: "OrganizationUsers");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_OrganizationId",
                table: "OrganizationUsers",
                column: "OrganizationId");
        }
    }
}
