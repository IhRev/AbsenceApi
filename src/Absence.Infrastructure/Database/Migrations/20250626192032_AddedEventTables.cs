using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedEventTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEventEntity_AbsenceEventTypeEntity_AbsenceEventTypeId",
                table: "AbsenceEventEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEventEntity_Organizations_OrganizationId",
                table: "AbsenceEventEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEventEntity_Users_UserId",
                table: "AbsenceEventEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbsenceEventTypeEntity",
                table: "AbsenceEventTypeEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbsenceEventEntity",
                table: "AbsenceEventEntity");

            migrationBuilder.RenameTable(
                name: "AbsenceEventTypeEntity",
                newName: "AbsenceEventTypes");

            migrationBuilder.RenameTable(
                name: "AbsenceEventEntity",
                newName: "AbsenceEvents");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEventEntity_UserId",
                table: "AbsenceEvents",
                newName: "IX_AbsenceEvents_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEventEntity_OrganizationId",
                table: "AbsenceEvents",
                newName: "IX_AbsenceEvents_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEventEntity_AbsenceEventTypeId",
                table: "AbsenceEvents",
                newName: "IX_AbsenceEvents_AbsenceEventTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbsenceEventTypes",
                table: "AbsenceEventTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbsenceEvents",
                table: "AbsenceEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEvents_AbsenceEventTypes_AbsenceEventTypeId",
                table: "AbsenceEvents",
                column: "AbsenceEventTypeId",
                principalTable: "AbsenceEventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEvents_Organizations_OrganizationId",
                table: "AbsenceEvents",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEvents_Users_UserId",
                table: "AbsenceEvents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ShortId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEvents_AbsenceEventTypes_AbsenceEventTypeId",
                table: "AbsenceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEvents_Organizations_OrganizationId",
                table: "AbsenceEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_AbsenceEvents_Users_UserId",
                table: "AbsenceEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbsenceEventTypes",
                table: "AbsenceEventTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbsenceEvents",
                table: "AbsenceEvents");

            migrationBuilder.RenameTable(
                name: "AbsenceEventTypes",
                newName: "AbsenceEventTypeEntity");

            migrationBuilder.RenameTable(
                name: "AbsenceEvents",
                newName: "AbsenceEventEntity");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEvents_UserId",
                table: "AbsenceEventEntity",
                newName: "IX_AbsenceEventEntity_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEvents_OrganizationId",
                table: "AbsenceEventEntity",
                newName: "IX_AbsenceEventEntity_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AbsenceEvents_AbsenceEventTypeId",
                table: "AbsenceEventEntity",
                newName: "IX_AbsenceEventEntity_AbsenceEventTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbsenceEventTypeEntity",
                table: "AbsenceEventTypeEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbsenceEventEntity",
                table: "AbsenceEventEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEventEntity_AbsenceEventTypeEntity_AbsenceEventTypeId",
                table: "AbsenceEventEntity",
                column: "AbsenceEventTypeId",
                principalTable: "AbsenceEventTypeEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEventEntity_Organizations_OrganizationId",
                table: "AbsenceEventEntity",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbsenceEventEntity_Users_UserId",
                table: "AbsenceEventEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ShortId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
