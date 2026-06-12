using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pulse.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitPriceToDoctorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VisitPrice",
                table: "DoctorProfiles",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitPrice",
                table: "DoctorProfiles");
        }
    }
}
