using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.API.Migrations
{
    /// <inheritdoc />
    public partial class MoveDoctorBranchVisitPriceToExtensionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitPrice",
                table: "Branches");

            migrationBuilder.CreateTable(
                name: "DoctorBranchProfiles",
                columns: table => new
                {
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorBranchProfiles", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_DoctorBranchProfiles_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorBranchProfiles");

            migrationBuilder.AddColumn<decimal>(
                name: "VisitPrice",
                table: "Branches",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
