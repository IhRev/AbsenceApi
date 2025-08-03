using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absence.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddInviter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUserInvitations_Users_UserId",
                table: "OrganizationUserInvitations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrganizationUserInvitations",
                newName: "Inviter");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUserInvitations_UserId",
                table: "OrganizationUserInvitations",
                newName: "IX_OrganizationUserInvitations_Inviter");

            migrationBuilder.AddColumn<int>(
                name: "Invited",
                table: "OrganizationUserInvitations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserInvitations_Invited",
                table: "OrganizationUserInvitations",
                column: "Invited");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUserInvitations_Users_Invited",
                table: "OrganizationUserInvitations",
                column: "Invited",
                principalTable: "Users",
                principalColumn: "ShortId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUserInvitations_Users_Inviter",
                table: "OrganizationUserInvitations",
                column: "Inviter",
                principalTable: "Users",
                principalColumn: "ShortId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUserInvitations_Users_Invited",
                table: "OrganizationUserInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationUserInvitations_Users_Inviter",
                table: "OrganizationUserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUserInvitations_Invited",
                table: "OrganizationUserInvitations");

            migrationBuilder.DropColumn(
                name: "Invited",
                table: "OrganizationUserInvitations");

            migrationBuilder.RenameColumn(
                name: "Inviter",
                table: "OrganizationUserInvitations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationUserInvitations_Inviter",
                table: "OrganizationUserInvitations",
                newName: "IX_OrganizationUserInvitations_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUserInvitations_Users_UserId",
                table: "OrganizationUserInvitations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ShortId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
