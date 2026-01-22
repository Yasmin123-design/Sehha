using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PharmaHub.Migrations
{
    /// <inheritdoc />
    public partial class tst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonorMatches");

            migrationBuilder.AddColumn<int>(
                name: "BloodRequestId",
                table: "DonorProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DonorProfiles_BloodRequestId",
                table: "DonorProfiles",
                column: "BloodRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_DonorProfiles_BloodRequests_BloodRequestId",
                table: "DonorProfiles",
                column: "BloodRequestId",
                principalTable: "BloodRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonorProfiles_BloodRequests_BloodRequestId",
                table: "DonorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DonorProfiles_BloodRequestId",
                table: "DonorProfiles");

            migrationBuilder.DropColumn(
                name: "BloodRequestId",
                table: "DonorProfiles");

            migrationBuilder.CreateTable(
                name: "DonorMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BloodRequestId = table.Column<int>(type: "int", nullable: false),
                    DonorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonorMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonorMatches_AspNetUsers_DonorUserId",
                        column: x => x.DonorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonorMatches_BloodRequests_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonorMatches_BloodRequestId",
                table: "DonorMatches",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DonorMatches_DonorUserId",
                table: "DonorMatches",
                column: "DonorUserId");
        }
    }
}
