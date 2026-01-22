using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationshipbetweenappuseranddonor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DonorProfiles_AppUserId",
                table: "DonorProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_DonorProfiles_AppUserId",
                table: "DonorProfiles",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DonorProfiles_AppUserId",
                table: "DonorProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_DonorProfiles_AppUserId",
                table: "DonorProfiles",
                column: "AppUserId",
                unique: true);
        }
    }
}
