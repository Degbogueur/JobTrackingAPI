using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToApplicationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Applications",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationDate",
                table: "Applications",
                column: "ApplicationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_CompanyName",
                table: "Applications",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_JobTitle",
                table: "Applications",
                column: "JobTitle");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Location",
                table: "Applications",
                column: "Location");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_ApplicationDate",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_CompanyName",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_JobTitle",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Location",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
