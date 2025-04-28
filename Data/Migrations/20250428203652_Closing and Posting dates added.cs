using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClosingandPostingdatesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterestRate",
                table: "Applications",
                newName: "InterestLevel");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosingDate",
                table: "Applications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostingDate",
                table: "Applications",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingDate",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PostingDate",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "InterestLevel",
                table: "Applications",
                newName: "InterestRate");
        }
    }
}
