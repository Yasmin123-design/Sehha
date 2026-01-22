using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class addbloodrequestcontroller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "DonorProfiles",
                newName: "DonorCountry");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "BloodRequests",
                newName: "NeedWithin");

            migrationBuilder.AddColumn<string>(
                name: "DonorCity",
                table: "DonorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "DonorLatitude",
                table: "DonorProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DonorLongitude",
                table: "DonorProfiles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "HospitalCity",
                table: "BloodRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HospitalCountry",
                table: "BloodRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "HospitalLatitude",
                table: "BloodRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "HospitalLongitude",
                table: "BloodRequests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DonorCity",
                table: "DonorProfiles");

            migrationBuilder.DropColumn(
                name: "DonorLatitude",
                table: "DonorProfiles");

            migrationBuilder.DropColumn(
                name: "DonorLongitude",
                table: "DonorProfiles");

            migrationBuilder.DropColumn(
                name: "HospitalCity",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "HospitalCountry",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "HospitalLatitude",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "HospitalLongitude",
                table: "BloodRequests");

            migrationBuilder.RenameColumn(
                name: "DonorCountry",
                table: "DonorProfiles",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "NeedWithin",
                table: "BloodRequests",
                newName: "City");
        }
    }
}
