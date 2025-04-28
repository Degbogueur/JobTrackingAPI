using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackingAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    ContractType = table.Column<int>(type: "int", nullable: false),
                    OfferUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverLetterFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastAction = table.Column<int>(type: "int", nullable: false),
                    LastActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextAction = table.Column<int>(type: "int", nullable: false),
                    NextActionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinSalaryProposed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxSalaryProposed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinSalaryOffered = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxSalaryOffered = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<int>(type: "int", nullable: true),
                    KeyWords = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterestRate = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
