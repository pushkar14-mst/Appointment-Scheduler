using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class C2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentModel_Users_UserModelId",
                table: "AppointmentModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentModel",
                table: "AppointmentModel");

            migrationBuilder.RenameTable(
                name: "AppointmentModel",
                newName: "Appointments");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentModel_UserModelId",
                table: "Appointments",
                newName: "IX_Appointments_UserModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UserModelId",
                table: "Appointments",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UserModelId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "AppointmentModel");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_UserModelId",
                table: "AppointmentModel",
                newName: "IX_AppointmentModel_UserModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentModel",
                table: "AppointmentModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentModel_Users_UserModelId",
                table: "AppointmentModel",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
